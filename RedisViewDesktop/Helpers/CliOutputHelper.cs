using StackExchange.Redis;
using System.Linq;
using System.Text;

namespace RedisViewDesktop.Helpers
{
    public class CliOutputHelper
    {
        private CliOutputHelper() { }

        public static object Format(RedisResult redisResult)
        {
            object result;
            if (redisResult.IsNull)
            {
                result = "(nil)";
            }
            else if (ResultType.Integer == redisResult.Type)
            {
                result = $"(integer){(long)redisResult}";
            }
            else if (ResultType.SimpleString == redisResult.Type)
            {
                result = (string)redisResult;
            }
            else if (ResultType.BulkString == redisResult.Type)
            {
                result = (string)redisResult;
            }
            else if (ResultType.MultiBulk == redisResult.Type)
            {
                result = FormatRedisArrayReply(redisResult);
            }
            else
            {
                result = redisResult;
            }
            return result;
        }

        private static string FormatRedisArrayReply(RedisResult redisResult, int level = 0)
        {
            string result = "";
            try
            {
                RedisResult[] results = (RedisResult[])redisResult;
                if (results?.Length == 0)
                {
                    result = "(empty list or set)";
                }
                else
                {
                    var strs = results?.Select((value, index) =>
                    {
                        string s = "\t";
                        string leftMargin = index > 0 ? Repeat(s, level) : "";
                        string lineIndex = $"{leftMargin}{index + 1})";
                        string v = FormatRedisArrayReply(value, level + 1);
                        return $"{lineIndex} {v}";
                    }).ToArray();
                    result = string.Join("\r\n", strs);
                }
            }
            catch
            {
                try
                {
                    var bytes = ((byte[]?)redisResult);
                    return FormatRedisBufferReply(bytes);
                }
                catch
                {
                    return JsonHelper.Serialization(redisResult);
                }
            }
            return result;
        }

        private static string Repeat(string value, int count)
        {
            if (count <= 0)
                return "";

            StringBuilder sb = new(value.Length * count);

            for (int i = 0; i < count; i++)
            {
                sb.Append(value);
            }

            return sb.ToString();
        }

        private static string FormatRedisBufferReply(byte[] bytes)
        {
            var result = "";
            foreach (var byteVal in bytes)
            {
                var charStr = Encoding.ASCII.GetString(new[] { byteVal });
                if (IsNonPrintableAsciiCharacter(charStr[0]))
                {
                    result += charStr; 
                }
                else
                {
                    result += charStr switch
                    {
                        "\\" => "\\\\",
                        "\u0007" => "\\a",
                        "\"" => "\\\"",
                        "\b" => "\\b",
                        "\t" => "\\t",
                        "\n" => "\\n",
                        "\r" => "\\r",
                        _ => charStr,
                    };
                }
            }
           
            return result;
        }

        private static bool IsNonPrintableAsciiCharacter(char c)
        {
            return !char.IsControl(c) && (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c));
        }

    }
}
