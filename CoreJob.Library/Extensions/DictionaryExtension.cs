using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CoreJob.Library.Extensions
{
	public static class DictionaryExtension
	{
		public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> dict)
		{
			var nvc = new NameValueCollection();

			dict.ToList().ForEach(d => nvc.Set(d.Key, d.Value));

			return nvc;
		}
	}
}
