using log4net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreJob.Library.Ticketing
{
	internal static class StartupTicketExtension
	{
		private static ILog logger = LogManager.GetLogger(typeof(StartupTicketExtension));

		public static void ScanTicketWorkers(this IServiceCollection services, IConfiguration configuration)
		{
			Settings.Current.Assemblies?.ToList().ForEach(a =>
			{
				try
				{
					var assembly = Assembly.Load(a);

					ScanTicketWorkers(assembly);
				}
				catch (Exception ex)
				{
					logger.Error($"Could not load Assembly {a}.", ex);
				}
			});
		}

		public static void ScanTicketWorkers(Assembly assembly)
		{
			var types = assembly.GetTypes()
							.Where(t => t.GetCustomAttribute<TicketWorkerAttribute>() != null
										&& t.IsAssignableTo(typeof(ITicketWorker))
										&& t.IsClass && !t.IsAbstract);

			foreach (var type in types)
			{
				var attr = type.GetCustomAttribute<TicketWorkerAttribute>();
				var registry = TicketWorkerRegistry.Instance;

				if (registry.ContainsKey(attr.TicketName))
				{
					logger.Warn($"Duplicate TicketName {attr.TicketName} worker. Old Type will be replaced by the new one.");
				}

				registry[attr.TicketName] = type;
			}
		}
	}
}
