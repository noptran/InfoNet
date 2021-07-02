// Pruned a cleansed version of Microsoft class TypeHelpers used in MVC binding.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.Mvc.Binding.Microsoft {
	internal static class TypeDescriptorHelper {
		public static ICustomTypeDescriptor Get(Type type) {
			return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
		}
	}
}