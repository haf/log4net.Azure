using System;
using System.ComponentModel;
using Microsoft.WindowsAzure.Diagnostics;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;

namespace log4net.Azure
{
	/// <summary>
	/// Configurator for the Azure Appender.
	/// </summary>
	public interface AzureAppenderConfigurator
	{
		/// <summary>
		/// Scheduled transfer period in milliseconds. Example:
		/// <code>conf.ScheduledTransferPeriod = 60000;</code>
		/// </summary>
		int ScheduledTransferPeriod { get; set; }

		/// <summary>
		/// Level of Logging. Example:
		/// <code>conf.Level = "Info";</code>
		/// </summary>
		string Level { get; set; }

		/// <summary>
		/// Gets the layout to render strings with. Example:
		/// <code>conf.Layout = new PatternLayout("%timestamp [%thread] %level %logger - %message%newline");</code>
		/// </summary>
		ILayout Layout { get; set; }

		/// <summary>
		/// Gets the semi-colon (;)-separated list of event logs to transfer. Example:
		/// <code>Application!*;System!*</code>
		/// </summary>
		string EventLogs { get; set; }

		/// <summary>
		/// Lets you configure the 'raw' appender. Avoid using this an instead
		/// expand upon this interface.
		/// </summary>
		/// <param name="appenderConfigurator">Lambda configuring appender.</param>
		void ConfigureInner(Action<AzureAppender> appenderConfigurator);

		/// <summary>
		/// Configure the repository hosting the appenders.
		/// </summary>
		/// <param name="repositoryConfigurator">Configurator, takes repository and level mapper :: string -> Level
		/// that can be used to set <see cref="ILoggerRepository.Threshold"/></param>
		void ConfigureRepository(Action<ILoggerRepository, Func<string, Level>> repositoryConfigurator);

		/// <summary>
		/// Configure azure diagnostics - this allows you to configure when and where to things
		/// get shipped.
		/// </summary>
		/// <param name="diagnosticsConfigurator">diagnostics configurator</param>
		void ConfigureAzureDiagnostics(Action<DiagnosticMonitorConfiguration> diagnosticsConfigurator);
	}
}