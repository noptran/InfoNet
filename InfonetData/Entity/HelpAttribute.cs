using System;
using Infonet.Data.Looking;

namespace Infonet.Data.Entity {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class HelpAttribute : Attribute {
		private readonly Provider _provider;
		private readonly string _text;

		// ReSharper disable once UnusedMember.Global
		public HelpAttribute(Provider provider, string text) {
			_provider = provider;
			_text = text;
		}

		// ReSharper disable once UnusedMember.Global
		public HelpAttribute(string text) {
			_provider = Provider.None;
			_text = text;
		}

		public Provider Provider {
			get { return _provider; }
		}

		public string Text {
			get { return _text; }
		}
	}
}