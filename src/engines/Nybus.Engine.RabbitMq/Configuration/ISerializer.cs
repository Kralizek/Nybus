using System;
using System.Text;
using INybusSerializer = Nybus.Serialization.ISerializer;

namespace Nybus.Configuration
{
    public interface ISerializer
    {
        byte[] SerializeObject(object item, Encoding encoding);

        object DeserializeObject(byte[] bytes, Type type, Encoding encoding);
    }

    public class NybusSerializerAdapter : ISerializer
    {
        private readonly INybusSerializer _serializer;

        public NybusSerializerAdapter(INybusSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public byte[] SerializeObject(object item, Encoding encoding) => _serializer.SerializeObject(item, encoding);

        public object DeserializeObject(byte[] bytes, Type type, Encoding encoding) => _serializer.DeserializeObject(bytes, type, encoding);
    }
}