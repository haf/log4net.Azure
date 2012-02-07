using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using log4net.Config;

namespace log4net.Azure.Sample
{
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

			return base.OnStart();
		}
	}
}