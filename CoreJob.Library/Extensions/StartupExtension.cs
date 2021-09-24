using CoreJob.Library.Ticketing;

using log4net;
using log4net.Appender;
using log4net.Config;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;
using Quartz.Impl;
using Quartz.Logging;

using System;
using System.IO;
using System.Linq;

namespace CoreJob.Library.Extensions
{
	public static class StartupExtension
	{
		private static ILog logger = LogManager.GetLogger(typeof(StartupExtension));

		public static async void AddCoreEngine(this IServiceCollection services, IConfiguration configuration)
		{
			Settings.LoadConfiguration(configuration);
			LogProvider.SetCurrentLogProvider(new CoreLogProvider());

			ConfigureLogAppender(configuration, Settings.Current);

			var settings = Settings.Current;
			// Grab the Scheduler instance from the Factory
			var factory = new StdSchedulerFactory();

			factory.Initialize(settings.QuartzProperties.ToNameValueCollection());

			var scheduler = await factory.GetScheduler();

			// register to ASP.NET DI
			services.AddSingleton(scheduler);
			services.AddSingleton(TicketWorkerRegistry.Instance);
			services.ScanTicketWorkers(configuration);

			// and start it off
			await scheduler.Start();

			var job = JobBuilder.Create<TicketWorkerJob>()
				.WithIdentity("TicketWorkerJob", "CoreJob")
				.Build();

			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x
					.WithIntervalInSeconds(5)
					.RepeatForever())
				.Build();

			if (!await scheduler.CheckExists(job.Key))
			{
				// Tell quartz to schedule the job using our trigger
				await scheduler.ScheduleJob(job, trigger);
			}

			settings.Jobs.ForEach(async jc =>
			{
				var myjob = JobBuilder.Create(jc.GetClassType())
								.WithIdentity(jc.Name, jc.Group)
								.WithDescription(jc.Description)
								.Build();
				var mytrigger = TriggerBuilder.Create()
								.WithCronSchedule(jc.Cron)
								.Build();

				jc.Parameters?.ToList()
					.ForEach(p =>
					{
						//trigBuilder.UsingJobData(p.Key, p.Value);
						myjob.JobDataMap.Add(p.Key, p.Value);
					});

				//var mytrigger = trigBuilder.Build();

				if (await scheduler.CheckExists(myjob.Key))
				{
					await scheduler.DeleteJob(myjob.Key);
				}

				await scheduler.ScheduleJob(myjob, mytrigger);

				logger.InfoFormat("Scheduled job '{0}' from group '{1}'", jc.Name, jc.Group);
			});
		}

		private static void ConfigureLogAppender(IConfiguration config, Settings settings)
		{
			var xml = Path.Combine(Directory.GetCurrentDirectory(), "log4net.xml");

			XmlConfigurator.ConfigureAndWatch(new FileInfo(xml));

			var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();

			var ado = hierarchy.Root.Appenders.ToArray()
						.Where(m => m is AdoNetAppender)
						.FirstOrDefault() as AdoNetAppender;

			if (ado == null || !string.IsNullOrWhiteSpace(ado.ConnectionString))
				return;

			var connName = ado.ConnectionStringName;

			if (!string.IsNullOrWhiteSpace(connName))
			{
				ado.ConnectionString = config.GetConnectionString(connName);
			}

			if (string.IsNullOrWhiteSpace(ado.ConnectionString))
			{
				ado.ConnectionString = settings.ConnectionString;
			}
		}
	}

	class CoreLogProvider : ILogProvider
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(CoreLogProvider));
		public Logger GetLogger(string name)
		{
			return (level, func, exception, parameters) =>
			{
				if (func == null)
					return true;

				switch (level)
				{
					case LogLevel.Debug:
						logger.Debug(string.Format(func(), parameters), exception);
						break;
					case LogLevel.Error:
						logger.Error(string.Format(func(), parameters), exception);
						break;
					case LogLevel.Fatal:
						logger.Fatal(string.Format(func(), parameters), exception);
						break;
					case LogLevel.Info:
						logger.Info(string.Format(func(), parameters), exception);
						break;
					case LogLevel.Trace:
						logger.Debug(string.Format(func(), parameters), exception);
						break;
					case LogLevel.Warn:
						logger.Warn(string.Format(func(), parameters), exception);
						break;
				}

				return true;
			};
		}

		public IDisposable OpenNestedContext(string message)
		{
			throw new NotImplementedException();
		}

		public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
		{
			throw new NotImplementedException();
		}
	}
}
