using System;
using System.IO;
using System.Collections.Specialized;
using System.Web.Management;
using System.Diagnostics;
using CMS;
using System.Text;

namespace System
{
	public static class InfoEventExtensionMethods
	{
		[DebuggerStepThrough]
		public static void RaiseInfoEvent(this object eventSource, string formatString, params object[] args)
		{
			new InfoEvent(string.Format(formatString, args), eventSource, 1000000).Raise();
		}

		[DebuggerStepThrough]
		public static void RaiseInfoEvent(this object eventSource, int eventCode, string formatString, params object[] args)
		{
			new InfoEvent(string.Format(formatString, args), eventSource, eventCode).Raise();
		}

		[DebuggerStepThrough]
		public static void RaiseInfoEvent(this object eventSource, Exception exception)
		{
			StringBuilder message = new StringBuilder();
			Exception ex = exception;
			while (ex != null)
			{
				message.AppendLine(ex.Message);
				ex = ex.InnerException;
			}
			message.AppendLine(exception.StackTrace);

			new InfoEvent(message.ToString(), eventSource, 1000001).Raise();
		}
	}
}

namespace CMS
{
	public class InfoEvent : System.Web.Management.WebBaseEvent
	{
		[DebuggerStepThrough]
		public InfoEvent(string message, object eventSource, int eventCode)
			: base(message, eventSource, eventCode)
		{
		}
	}

	public class LogFileWebEventProvider : BufferedWebEventProvider
	{
		object fileLock = new object();
		string fileName;

		[DebuggerStepThrough]
		public override void Initialize(string name, NameValueCollection config)
		{
			fileName = config.Get("fileName");
			if (!Path.IsPathRooted(fileName))
			{
				fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
			}
			config.Remove("fileName");
			base.Initialize(name, config);
		}

		[DebuggerStepThrough]
		public override void ProcessEvent(WebBaseEvent raisedEvent)
		{
			if (this.UseBuffering)
			{
				base.ProcessEvent(raisedEvent);
			}
			else
			{
				lock (fileLock)
				{
					string file = string.Format(fileName, DateTime.Now);
					bool header = !File.Exists(file);
					using (StreamWriter sw = File.AppendText(file))
					{
						WriteEvent(sw, header, raisedEvent);
					}
				}
			}
		}

		[DebuggerStepThrough]
		public override void ProcessEventFlush(WebEventBufferFlushInfo info)
		{
			lock (fileLock)
			{
				string file = string.Format(fileName, DateTime.Now);
				bool header = !File.Exists(file);
				using (StreamWriter sw = File.AppendText(file))
				{
					foreach (WebBaseEvent e in info.Events)
					{
						WriteEvent(sw, header, e);
					}
				}
			}
		}

		[DebuggerStepThrough]
		internal static void WriteEvent(TextWriter tw, bool header, WebBaseEvent raisedEvent)
		{
			if (header)
			{
				// Header
				tw.WriteLine("DateTime\tSource\tCode\tMessage");
			}

			tw.WriteLine("{0:yyyyMMdd.HHmmss}\t{1}\t{2}\t{3}",
				raisedEvent.EventTime,
				raisedEvent.EventSource.GetType().Name,
				raisedEvent.EventCode,
				raisedEvent.Message);
		}
	}

}