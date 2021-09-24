using System.Collections.Generic;
using System.Text;

namespace CoreJob.Library.Utils
{
	public class TemplateUtil
	{
		public static string Parse(string template, IDictionary<string, string> values)
		{
			if (string.IsNullOrWhiteSpace(template))
				return template;

			if (values == null)
				return template;

			var sb = new StringBuilder(template);

			foreach(var prop in values)
			{
				var propName = prop.Key;
				var value = prop.Value;

				sb.Replace("@" + propName + "@", value);
			}

			return sb.ToString();
		}
	}
}
