using NUnit.Framework;
using XmlRepository.DataProviders;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests.DataProviders
{
    [TestFixture]
    public class InMemoryDataProviderTest
    {
        [Test]
        public void WhenConstructedWithInitialNullOrEmptyDataContent_DefaultRootElementIsReturned()
        {
            var provider = new InMemoryDataProvider(null);

            Assert.That(provider.Load<Person>(), Is.EqualTo(XmlRepository.RootElementXml));

            provider = new InMemoryDataProvider(string.Empty);

            Assert.That(provider.Load<Person>(), Is.EqualTo(XmlRepository.RootElementXml));
        }
    }
}