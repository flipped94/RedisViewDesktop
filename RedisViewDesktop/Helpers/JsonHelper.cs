using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedisViewDesktop.Helpers
{
    public class JsonHelper
    {
        public static string Format(string str)
        {
            try
            {
                JToken token = JToken.Parse(str);
                return token.ToString(Formatting.Indented);
            }
            catch
            {

            }
            return str;
        }

        public static string Serialization(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
