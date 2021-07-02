using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace Infonet.Core.Logging {
	/** Simple logging mechanism.  Threadsafe but not particularly process safe (i.e. multiple processes can't log to same file.) **/
	public static class Log {
		private static readonly string _FileNameTemplate = ConfigurationManager.AppSettings["LogFile"];
		private static readonly object _WriteLock = new object();
		private static StreamWriter _Writer = null;
		private static string _WriterFileName = null;

		public static void Debug(string message, params object[] args) {
			if (args != null && args.Length > 0)
				message = string.Format(message, args);
			lock (_WriteLock) {
				var now = DateTime.Now;
				string fileName = string.Format(_FileNameTemplate, now);
				if (fileName != _WriterFileName) {
					_Writer?.Dispose();
					_Writer = new StreamWriter(new FileStream(_WriterFileName = fileName, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
				}
				_Writer.WriteLine($"{now}\t[{Thread.CurrentThread.ManagedThreadId}]\t{message}");
				_Writer.Flush();
			}
		}

		public static void Debug(string source, MailMessage message) {
			Debug($"{source} sending email:{Environment.NewLine}\tFrom: {message.From}{Environment.NewLine}\tTo: {message.To}{Environment.NewLine}\tSubject: {message.Subject}{Environment.NewLine}\tBody: {message.Body}");
		}
	}
}