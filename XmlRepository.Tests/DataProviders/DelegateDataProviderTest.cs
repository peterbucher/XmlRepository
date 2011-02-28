using NUnit.Framework;
using XmlRepository.DataProviders;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests.DataProviders
{
    [TestFixture]
    public class DelegateDataProviderTest
    {
        [Test]
        public void WithEmptyStringDelegate_EmptyRootElementIsReturned()
        {
            string dataReference = string.Empty;
            var provider = new DelegateDataProvider(
                () => dataReference,
                data => dataReference = data);

            Assert.That(provider.Load<Person>(), Is.EqualTo(XmlRepository.RootElementXml));
        }

        [Test]
        public void WithCorrectStringDelegate_SameValueIsReturned()
        {
            string dataReference =
                "<root></root>";

            var provider = new DelegateDataProvider(
                () => dataReference,
                data => dataReference = data);

            Assert.That(provider.Load<Person>(), Is.EqualTo(dataReference));
        }

        [Test]
        public void WithManipulatedInput_ThisInputWillOnLoadReturned()
        {
            string dataReference =
                "<root></root>";

            var provider = new DelegateDataProvider(
                () => dataReference,
                data => dataReference = data);

            string newInput = "<root><Person /></root>";

            provider.Save<Person>(newInput);

            Assert.That(provider.Load<Person>(), Is.EqualTo(newInput));
            Assert.That(dataReference, Is.EqualTo(newInput));
        }
    }
}