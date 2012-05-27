# log4net.Azure

An appender that plugs into the Trace-pipeline w/ the logging that Windows Azure uses.

## Requirements:

 * .Net 4.0
 * libs from above

## Using it:

First, install it; `install-package log4net.Azure`

Secondly, use it;

```
public class Log4NetSampleWorker : RoleEntryPoint
{
	private static readonly ILog _logger = LogManager.GetLogger(typeof (Log4NetSampleWorker));

	public override void Run()
	{
		_logger.InfoFormat("{0}'s entry point called", typeof(Log4NetSampleWorker).Name);

		while (true)
		{
			Thread.Sleep(10000);
			_logger.Debug("Working...");
		}
	}

	public override bool OnStart()
	{
		BasicConfigurator.Configure(AzureAppender.New(conf =>
			{
				conf.Level = "Debug";
				conf.ConfigureRepository((repo, mapper) =>
					{
						repo.Threshold = mapper("Debug"); // root (defaults to 'All')
					});
			}));

		return base.OnStart();
	}
}
```

The configurator gives you access to doing things like this as well;

```
BasicConfigurator.Configure(AzureAppender.New(conf =>
	{
		/* in this case we say that appender should be in debug mode
			* but that we should only transfer logs to storage that is information-level
			* or above.
			* 
			* That doesn't make sense unless you're also doing debugging and want the debug
			* output in the trace viewer or you have another trace message sink.
			* 
			* Please note the wonderful *three* types of logging levels here! Nice. Not to
			* mention that you can do the equivalent of conf.Level = "Debug" in the cscfg file.
			*/

		conf.Level = "Debug";

		conf.ConfigureRepository((repo, mapper) =>
			{
				repo.Threshold = mapper("Debug"); // root
			});

		conf.ConfigureAzureDiagnostics(dmc =>
			{
				dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Information;
			});
	}));
```

## Building:

 1. Install libs: http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=28045
 2. Run 'gem install bundler' (if not already installed)
 2. Run `rake`.

Assumes your diagnostics connection string is named: `Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString`. It turns out one can't change this in the cscfg files in VS.

Optional Settings (that can also be set through the `ConfigureAzureDiagnostics` method call):

 * `Diagnostics.Level` - [level of logging](http://logging.apache.org/log4net/release/manual/introduction.html), unless configured in code. Accepts { 'Debug', 'Info', 'Warn', 'Error', 'Fatal', 'Off', 'All' }. Defaults to 'Info'.
 * `Diagnostics.Layout` - [pattern layout](http://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html), unless configured in code. Defaults to `"%timestamp [%thread] %level %logger - %message%newline"`
 * `Diagnostics.ScheduledTransferPeriod` - transfer period, unless configured in code. [ms] Defaults to 60000 ms.
 * `Diagnostics.EventLogs` - event-logs key. Defaults to `Application!*;System!*`

Want a feature? Put up an issue and send a pull request!

Some code from here:
Ref: http://cloudshaper.wordpress.com/2010/10/30/logging-with-log4net-on-the-azure-platform/