using System;
using System.Text;
using INybusSerializer = Nybus.Serialization.ISerializer;

namespace Nybus.Configuration
{
    public interface ISerializer
    {
        string SerializeObject(object item);

        object DeserializeObject(string item, Type type);
    }

    public class NybusSerializerAdapter : ISerializer
    {
        private readonly INybusSerializer _serializer;
        private readonly Encoding _encoding;

        public NybusSerializerAdapter(INybusSerializer serializer, Encoding encoding)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public string SerializeObject(object item) => _encoding.GetString(_serializer.SerializeObject(item, _encoding));

        public object DeserializeObject(string item, Type type) => _serializer.DeserializeObject(_encoding.GetBytes(item), type, _encoding);
    }
}