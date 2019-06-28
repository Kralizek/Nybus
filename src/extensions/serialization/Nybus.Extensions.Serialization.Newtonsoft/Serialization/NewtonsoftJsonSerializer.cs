using System;
using System.Text;
using Newtonsoft.Json;

namespace Nybus.Serialization
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public byte[] SerializeObject(object item, Encoding encoding)
        {
            var json = JsonConvert.SerializeObject(item, _settings);
            return encoding.GetBytes(json);
        }

        public object DeserializeObject(byte[] bytes, Type type, Encoding encoding)
        {
            var item = encoding.GetString(bytes);
            return JsonConvert.DeserializeObject(item, type, _settings);
        }
    }
}
