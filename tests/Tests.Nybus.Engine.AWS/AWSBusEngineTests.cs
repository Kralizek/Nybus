using AutoFixture;
using AutoFixture.Idioms;
using NUnit.Framework;
using Nybus;

namespace Tests
{
    [TestFixture]
    public class AWSBusEngineTests
    {
        [Test, AutoMoqData]
        public void Constructors_are_correctly_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(AWSBusEngine).GetConstructors());
        }
    }
}