using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace log4net.Azure
{
	/// <summary>
	/// Appender that writes to the trace.
	/// </summary>
	public class AzureAppender : AppenderSkeleton, AzureAppenderConfigurator
	{
		public const string ConnectionStringKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
		public const string LevelKey = "Diagnostics.Level";
		public const string LayoutKey = "Diagnostics.Layout";
		public const string ScheduledTransferPeriodKey = "Diagnostics.ScheduledTransferPeriod";
		public const string EventLogsKey = "Diagnostics.EventLogs";

		public const string DefaultPatternLayout = "%timestamp [%thread] %level %logger - %message%newline";
		public const string DefaultEventLogsPattern = "Application!*;System!*";

		public AzureAppender()
		{
			// workaround; overriding fails, because it targets its field
			Layout = GetLayout();
		}

		private static ILayout GetLayout()
		{
			try { return new PatternLayout(RoleEnvironment.GetConfigurationSettingValue(LayoutKey)); }
			catch (Exception) { return new PatternLayout(DefaultPatternLayout); }
		}


		#region Settings

		private int _scheduledTransferPeriod = GetScheduledTransferPeriod(); // 1 min

		private static int GetScheduledTransferPeriod()
		{
			try { return int.Parse(RoleEnvironment.GetConfigurationSettingValue(ScheduledTransferPeriodKey)); }
			catch (Exception) { return 60000; }
		}

		/// <summary>
		/// Scheduled transfer period in milliseconds
		/// </summary>
		public int ScheduledTransferPeriod
		{
			get { return _scheduledTransferPeriod; }
			set { _scheduledTransferPeriod = value; }
		}

		private string _level = GetLevel();

		private static string GetLevel()
		{
			try { return RoleEnvironment.GetConfigurationSettingValue(LevelKey); }
			catch (Exception) { return "Info"; }
		}

		/// <summary>
		/// Level of Logging.
		/// </summary>
		public string Level
		{
			get { return _level; }
			set { _level = value; }
		}

		private string _eventLogs = GetEventLogs();

		private static string GetEventLogs()
		{
			try { return RoleEnvironment.GetConfigurationSettingValue(EventLogsKey); }
			catch (Exception) { return DefaultEventLogsPattern; }
		}

		/// <summary>
		/// Gets the semi-colon (;)-separated list of event logs to transfer.
		/// </summary>
		public string EventLogs
		{
			get { return _eventLogs; }
			set { _eventLogs = value; }
		}

		void AzureAppenderConfigurator.ConfigureInner(Action<AzureAppender> appender)
		{
			if (appender == null) throw new ArgumentNullException("appender");
			appender(this);
		}

		#endregion

		protected override void Append(LoggingEvent loggingEvent)
		{
			var logString = RenderLoggingEvent(loggingEvent);

			Trace.Write(logString);
		}

		public override void ActivateOptions()
		{
			ConfigureThreshold();

			base.ActivateOptions();

			var dmc = ConfigureAzureDiagnostics();

			StartAzureDiagnostics(dmc);
		}

		private void ConfigureThreshold()
		{
			Threshold = ((Hierarchy) LogManager.GetRepository()).LevelMap[Level];
		}

		private DiagnosticMonitorConfiguration ConfigureAzureDiagnostics()
		{
			var dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
			dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

			ScheduleTransfer(dmc, ScheduledTransferPeriod);
			ConfigureWindowsEventLogsToBeTransferred(dmc, EventLogs);

			return dmc;
		}

		/// <summary>
		/// Starts azure diagnostics
		/// </summary>
		private static void StartAzureDiagnostics(DiagnosticMonitorConfiguration dmc)
		{
			Trace.Listeners.Add(new DiagnosticMonitorTraceListener());
			DiagnosticMonitor.Start(ConnectionStringKey, dmc);
		}

		private static void ScheduleTransfer(DiagnosticMonitorConfiguration dmc, int transferPeriodMs)
		{
			var transferPeriod = TimeSpan.FromMilliseconds(transferPeriodMs);
			dmc.Logs.ScheduledTransferPeriod = transferPeriod;
			dmc.WindowsEventLog.ScheduledTransferPeriod = transferPeriod;
		}

		private static void ConfigureWindowsEventLogsToBeTransferred(DiagnosticMonitorConfiguration dmc, string eventLogs)
		{
			var logs = eventLogs.Split(';');
			
			foreach (var log in logs)
				dmc.WindowsEventLog.DataSources.Add(log);
		}

		/// <summary>
		/// Creates a new AzureAppender and activates its options, allowing you to pass
		/// a configurator, to configure the appender, before activation.
		/// </summary>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static AzureAppender New(Action<AzureAppenderConfigurator> configurator)
		{
			if (configurator == null) throw new ArgumentNullException("configurator");
			var appender = new AzureAppender();
			configurator(appender);
			appender.ActivateOptions();
			return appender;
		}
	}
}