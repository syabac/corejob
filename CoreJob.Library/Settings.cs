using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
namespace CoreJob.Library
{
	public class Settings
	{
		private Dictionary<string, string> quartzProps = new();
		public IConfiguration Configuration { get; private set; }
		public string ConnectionString { get; set; }
		public string DataDirectory { get; set; }
		/// <summary>
		/// Additional options stored here
		/// </summary>
		public Dictionary<string, object> Options { get; set; }
		public Dictionary<string, string> QuartzProperties
		{
			get
			{
				quartzProps.TryGetValue("quartz.jobStore.dataSource", out string dsName);

				if (string.IsNullOrWhiteSpace(dsName))
					return quartzProps;

				var propName = string.Format("quartz.dataSource.{0}.connectionString", dsName);

				if (quartzProps.ContainsKey(propName))
					return quartzProps;

				quartzProps[propName] = ConnectionString;

				return quartzProps;
			}
			set
			{
				quartzProps = value;
			}
		}
		public List<JobConfig> Jobs { get; set; } = new();
		public SmtpSetting SmtpSetting { get; set; }
		public IList<string> Assemblies { get; set; }

		public static Settings Current { get; private set; }

		public static void LoadConfiguration(IConfiguration config)
		{
			Current = config.GetSection("CoreJob").Get<Settings>();
			Current.Configuration = config;
		}

	}

}
