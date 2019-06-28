using System;
using System.Text;

namespace Nybus.Serialization
{
    public interface ISerializer
    {
        byte[] SerializeObject(object item, Encoding encoding);

        object DeserializeObject(byte[] bytes, Type type, Encoding encoding);
    }
}