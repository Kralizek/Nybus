using System;
using System.Text;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using Nybus.Configuration;
using Nybus.Serialization;
using ISerializer = Nybus.Serialization.ISerializer;

namespace Tests.Configuration
{
    [TestFixture]
    public class NybusSerializerAdapterTests
    {
        [Test, CustomAutoMoqData]
        public void SerializeObject_forwards_to_inner_serializer([Frozen] ISerializer serializer, NybusSerializerAdapter sut, FirstTestCommand testCommand, Encoding encoding)
        {
            var result = sut.SerializeObject(testCommand, encoding);

            Mock.Get(serializer).Verify(p => p.SerializeObject(testCommand, encoding));
        }

        [Test, CustomAutoMoqData]
        public void SerializeObject_forwards_to_inner_serializer([Frozen] ISerializer serializer, NybusSerializerAdapter sut, FirstTestEvent testEvent, Encoding encoding)
        {
            var result = sut.SerializeObject(testEvent, encoding);

            Mock.Get(serializer).Verify(p => p.SerializeObject(testEvent, encoding));
        }

        [Test, CustomAutoMoqData]
        public void DeserializeObject_forwards_to_inner_serializer([Frozen] ISerializer serializer, NybusSerializerAdapter sut, byte[] bytes, Encoding encoding, FirstTestCommand testCommand)
        {
            Mock.Get(serializer).Setup(p => p.DeserializeObject(It.IsAny<byte[]>(), It.IsAny<Type>(), It.IsAny<Encoding>())).Returns(testCommand);

            var result = sut.DeserializeObject(bytes, typeof(FirstTestCommand), encoding);

            Assert.That(result, Is.InstanceOf<FirstTestCommand>());
        }

        [Test, CustomAutoMoqData]
        public void DeserializeObject_forwards_to_inner_serializer([Frozen] ISerializer serializer, NybusSerializerAdapter sut, byte[] bytes, Encoding encoding, FirstTestEvent testEvent)
        {
            Mock.Get(serializer).Setup(p => p.DeserializeObject(It.IsAny<byte[]>(), It.IsAny<Type>(), It.IsAny<Encoding>())).Returns(testEvent);

            var result = sut.DeserializeObject(bytes, typeof(FirstTestEvent), encoding);

            Assert.That(result, Is.InstanceOf<FirstTestEvent>());
        }

    }
}
