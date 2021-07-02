using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using Infonet.Core.Collections;
using Infonet.Web.Mvc.Binding.Microsoft;

namespace Infonet.Web.Mvc.Binding {
	public class DerivedDictionaryRule : IModelBindingRule {
		// ReSharper disable MemberCanBePrivate.Global

		public const string INDEX_PROPERTY = "index"; /* hardcoded in javascript */
		public const string KEY_ADD_PREFIX = "+";
		public const string KEY_REMOVE_PREFIX = "-";
		public const string KEY_PREFIX = "=";
		public const string KEY_ADD_REMOVE_PREFIX = "~";
		public const int KEY_PREFIX_LENGTH = 1;

		// ReSharper restore MemberCanBePrivate.Global

		public bool AppliesTo(ModelBindingContext bindingContext) {
			var type = TypeHelpers.ExtractGenericInterface(bindingContext.ModelType, typeof(DerivedDictionary<>));
			return type != null;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			object model = bindingContext.Model;
			Type modelType = bindingContext.ModelType;

			if (model == null)
				throw new InvalidOperationException(GetType() + " does not support null models");

			Type elementType = TypeHelpers.ExtractGenericInterface(bindingContext.ModelType, typeof(DerivedDictionary<>)).GetGenericArguments()[0];

			/* this doesn't appear to be any different from bindingCotext parameter, but DefaultModelBinder does this so we will too. */
			//KMS DO does the new context help when the model is null?
			var collectionBindingContext = new ModelBindingContext {
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				PropertyFilter = bindingContext.PropertyFilter,
				ValueProvider = bindingContext.ValueProvider
			};

			var result = BindElements(controllerContext, collectionBindingContext, elementType);
			//KMS DO maybe useful if we supported null models...not sure
			//bindingContext.ModelState.SetModelValue(bindingContext.ModelName, null /* result.ValueProviderResult */);
			return result;
		}

		private object BindElements(ControllerContext controllerContext, ModelBindingContext bindingContext, Type elementType) {
			var collection = (IKeyAccess)bindingContext.Model;
			var elementBinder = ModelBinders.Binders.GetBinder(elementType);

			foreach (var each in GetIndexes(bindingContext)) {
				string subIndexPath = CreateKeyPath(bindingContext.ModelName, each.Value);
				if (!bindingContext.ValueProvider.ContainsPrefix(subIndexPath))
					throw new Exception("No values to bind for prefix " + subIndexPath); /* perhaps we should just let this go */

				Func<object> elementAccessor = () => collection[each.IsAdd ? Key.Template() : each.Value];
				var innerContext = new ModelBindingContext {
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(elementAccessor, elementType),
					ModelName = subIndexPath,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				object boundElement = elementBinder.BindModel(controllerContext, innerContext);

				AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, subIndexPath, elementType, boundElement);

				if (each.IsAdd)
					collection.Add(each.Value, boundElement);
				if (each.IsRemove) {
					collection.Remove(each.Value);
					foreach (string key in bindingContext.ModelState.Keys)
						if (key.StartsWith(subIndexPath)) {
							var value = bindingContext.ModelState[key];
							value.Errors.Clear();
						}
				}
			}
			return collection;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private static void AddValueRequiredMessageToModelState(ControllerContext controllerContext, ModelStateDictionary modelState, string modelStateKey, Type elementType, object value) {
			if (value == null && !TypeHelpers.TypeAllowsNullValue(elementType) && modelState.IsValidField(modelStateKey))
				modelState.AddModelError(modelStateKey, "A value is required.");
		}

		#region create paths
		//KMS DO can these be elimnated (or moved elsewhere) later?
		private static string CreatePropertyPath(string prefix, string propertyName) {
			if (string.IsNullOrEmpty(prefix))
				return propertyName;
			if (string.IsNullOrEmpty(propertyName))
				return prefix;
			return prefix + "." + propertyName;
		}

		private static string CreateKeyPath(string prefix, object index) {
			return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
		}
		#endregion

		#region pseudo property values
		private static Index[] GetIndexes(ModelBindingContext bindingContext) {
			string[] rawValues = GetPropertyValues(bindingContext, INDEX_PROPERTY, false);
			Index[] result = new Index[rawValues.Length];
			for (int i = 0; i < result.Length; i++)
				result[i] = new Index(rawValues[i]);
			Array.Sort(result);
			return result;
		}

		public static string[] GetPropertyValues(ModelBindingContext bindingContext, string propertyName, bool required) {
			string subProperty = CreatePropertyPath(bindingContext.ModelName, propertyName);

			string[] result = null;
			var valueProviderResult = bindingContext.ValueProvider.GetValue(subProperty);
			if (valueProviderResult != null)
				result = valueProviderResult.ConvertTo(typeof(string[])) as string[];

			if (result != null)
				return result;

			if (required)
				throw new Exception("Request missing " + subProperty);

			return Array.Empty<string>();
		}

		private struct Index : IComparable<Index> {
			public Index(string rawValue) {
				if (rawValue == null)
					throw new ArgumentNullException(nameof(rawValue));

				Prefix = rawValue.Substring(0, Math.Min(KEY_PREFIX_LENGTH, rawValue.Length));
				Value = Key.Parse(rawValue.Substring(KEY_PREFIX_LENGTH));

				if (Prefix != KEY_PREFIX && Prefix != KEY_REMOVE_PREFIX && Prefix != KEY_ADD_PREFIX && Prefix != KEY_ADD_REMOVE_PREFIX)
					throw new ArgumentException($"Indexes must begin with {KEY_ADD_PREFIX}, {KEY_REMOVE_PREFIX}, {KEY_ADD_REMOVE_PREFIX}, or {KEY_PREFIX}", nameof(rawValue));
			}

			public Key Value { get; }

			[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
			public string Prefix { get; }

			public bool IsAdd {
				get { return Prefix == KEY_ADD_PREFIX || Prefix == KEY_ADD_REMOVE_PREFIX; }
			}

			public bool IsRemove {
				get { return Prefix == KEY_REMOVE_PREFIX || Prefix == KEY_ADD_REMOVE_PREFIX; }
			}

			public int CompareTo(Index other) {
				int result = Value.CompareTo(other.Value);
				if (result != 0)
					return result;

				return string.Compare(other.Prefix, Prefix, StringComparison.Ordinal);
			}
		}
		#endregion
	}
}