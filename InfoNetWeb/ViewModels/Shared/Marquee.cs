using System;

namespace Infonet.Web.ViewModels.Shared {
	public class Marquee {
		public DateTime RefreshedAt { get; set; }
		public Message[] Messages { get; set; }

		public class Message {
			public string Title { get; set; }
			public string Text { get; set; }
			public bool IsHot { get; set; }
		}
	}
}