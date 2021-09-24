using log4net;

using System;
using System.Collections.Generic;

namespace CoreJob.Library.Ticketing
{
	/// <summary>
	/// <![CDATA[
	/// Class that responsible register TicketWorkers type
	/// 
	/// Author: Syamsul Bachri <syabac@gmail.com>
	/// ]]>
	/// 
	/// </summary>
	public class TicketWorkerRegistry : Dictionary<string, Type>
	{
		private static ILog logger = LogManager.GetLogger(typeof(TicketWorkerRegistry));
		public static TicketWorkerRegistry Instance { get; private set; } = new TicketWorkerRegistry();

		private TicketWorkerRegistry() : base() { }

		public ITicketWorker GetWorker(string ticketName)
		{
			if (!ContainsKey(ticketName))
			{
				logger.Warn($"Could not find worker from registry for TicketName {ticketName}");
				return null;
			}

			var type = this[ticketName];

			try
			{
				return Activator.CreateInstance(type) as ITicketWorker;
			}
			catch (Exception ex)
			{
				logger.Error($"Could not instantiate TicketWorker {ticketName} with type {type.FullName}", ex);
			}

			return null;
		}
	}
}
