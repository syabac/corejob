using log4net;

using Quartz;

using System;
using System.Collections.Generic;

namespace CoreJob.Library
{
	public class JobConfig
	{
		private static ILog log = LogManager.GetLogger(typeof(JobConfig));

		public string Name { get; set; }
		public string Group { get; set; }
		public string Description { get; set; }
		public string Cron { get; set; }
		public string ClassName { get; set; }
		public Dictionary<string, dynamic> Parameters { get; set; }

		public Type GetClassType()
		{
			var type = Type.GetType(ClassName);

			if (type == null)
			{
				log.ErrorFormat("Could'nt find Job Class {0}", ClassName);
				return null;
			}

			if (!typeof(IJob).IsAssignableFrom(type) || type.IsAbstract)
			{
				log.ErrorFormat("Job Class {0} is not implements Quartz.IJob interface", ClassName);
				return null;
			}

			return type;
		}
	}
}
