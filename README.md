# log4net.Azure

An appender that plugs into the Trace-pipeline w/ the logging that Windows Azure uses.

Code comes from http://cloudshaper.wordpress.com/2010/10/30/logging-with-log4net-on-the-azure-platform/

Benefits of this project: 

 * A real repository that can be pushed to (I haven't been able to find theirs)
 * NuGet-it should be a package.
 
## Building:

 1. Install libs: http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=28045
 2. Run `rake`.

## Requirements:

 * .Net 4.0

## Using it:

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
			}));

		return base.OnStart();
	}
}
```

Assumes your diagnostics connection string is named: `Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString`.

Optional Settings:

 * **`Diagnostics.Level`** - [level of logging](http://logging.apache.org/log4net/release/manual/introduction.html), unless configured in code. Accepts { 'Debug', 'Info', 'Warn', 'Error', 'Fatal', 'Off', 'All' }. Defaults to 'Info'.
 * **`Diagnostics.Layout`** - [pattern layout](http://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html), unless configured in code. Defaults to `"%timestamp [%thread] %level %logger - %message%newline"`
 * **`Diagnostics.ScheduledTransferPeriod`** - transfer period, unless configured in code. [ms] Defaults to 60000 ms.
 * **`Diagnostics.EventLogs`** - event-logs key. Defaults to `Application!*;System!*`

Want a feature? Put up an issue and send a pull request!