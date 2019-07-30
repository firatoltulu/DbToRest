using Newtonsoft.Json.Linq;
using System;

namespace DbToRest.Core
{
    public static class JsonExtension
    {
        public static void AddField(this JToken model, string name, object value)
        {
            model[name] = JValue.FromObject(value);
        }

        public static void SelectToken(this JToken model, string expression, string name, object value)
        {
            JToken _token = model.SelectToken(expression);
            if (_token.HasValues)
            {
                _token[name] = JValue.FromObject(value);
            }
        }

        public static bool HasValue(this JToken token)
        {
            return !((token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null));
        }
    }
}