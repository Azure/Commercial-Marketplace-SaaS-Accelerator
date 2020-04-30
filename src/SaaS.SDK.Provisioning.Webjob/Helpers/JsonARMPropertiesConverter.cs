using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Helpers
{
    public class JsonARMPropertiesConverter : JsonConverter
    {
        private readonly Type[] _types;

        public JsonARMPropertiesConverter(params Type[] types)
        {
            _types = types;
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            JObject input = (JObject)t;

            JObject output = new JObject();

            var properties = input.Children().ToList();
            foreach (var p in properties)
            {
                dynamic jsonObject = new JObject();
                jsonObject.value = ((JProperty)p).Value;
                output.Add(((JProperty)p).Name, jsonObject);
            }

            output.WriteTo(writer);
        }

        public override bool CanRead
        {
            get { return false; }
        }

    }
}
