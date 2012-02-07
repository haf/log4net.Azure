using System.Diagnostics;
using System.Threading;
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
					conf.Level = "Debug";
					conf.ConfigureRepository((repo, mapper) =>
						{
							repo.Threshold = mapper("Debug"); // root
						});
				}));

			return base.OnStart();
		}
	}
}