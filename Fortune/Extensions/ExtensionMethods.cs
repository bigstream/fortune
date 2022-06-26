using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortune.Extensions
{
    public static class ExtensionMethods
    {
        public static T DeepClone<T>(this T original)
        {
            var tmp = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<T>(tmp);
        }
    }
}
