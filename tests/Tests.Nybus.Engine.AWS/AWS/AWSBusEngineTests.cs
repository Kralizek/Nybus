using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using NUnit.Framework;
using Nybus;
using Nybus.AWS;
using System.Reactive.Linq;

namespace Tests.AWS
{
    [TestFixture]
    public class AWSBusEngineTests
    {
        [Test, AutoMoqData]
        public void Constructors_are_correctly_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(AWSBusEngine).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Start_returns_closed_sequence_if_no_subscription(AWSBusEngine sut)
        {
            var sequence = await sut.StartAsync();

            var items = sequence.DumpInList();

            Assert.That(items, Is.Empty);
        }
    }
}