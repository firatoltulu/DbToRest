using Newtonsoft.Json;

namespace DbToRest.Core
{
    public static class Serialize
    {
        public static T FromJson<T>(string input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var serialize = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);
            return serialize;
        }

        public static string ToJson(object input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return Newtonsoft.Json.JsonConvert.SerializeObject(input);
        }
    }
}