using System;
using System.Linq;
using NUnit.Framework;
using XmlRepository.DataProviders;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests
{
    [TestFixture]
    public class XmlRepositoryTests
    {
        [Test]
        public void IntegrationTest()
        {
            var peter =
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Peter",
                    LastName = "Bucher",
                    Birthday = new DateTime(1983, 10, 17)
                };
            var golo =
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Golo",
                    LastName = "Roden",
                    Birthday = new DateTime(1978, 9, 27)
                };

            XmlRepository.DataProvider = new XmlInMemoryProvider();
            using (var firstRepository = XmlRepository.GetInstance<Person>())
            {
                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(() => firstRepository.LoadBy(p => p.Id == peter.Id), Throws.Exception);
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.SaveOnSubmit(peter);

                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(firstRepository.LoadBy(p => p.Id == peter.Id).LastName, Is.EqualTo(peter.LastName));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.DiscardChanges();

                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(() => firstRepository.LoadBy(p => p.Id == peter.Id), Throws.Exception);
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.SaveOnSubmit(peter);

                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(firstRepository.LoadBy(p => p.Id == peter.Id).LastName, Is.EqualTo(peter.LastName));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.SubmitChanges();

                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(firstRepository.LoadBy(p => p.Id == peter.Id).LastName, Is.EqualTo(peter.LastName));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.DiscardChanges();

                Assert.That(firstRepository.LoadAll().Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(1));
                Assert.That(firstRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(0));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
                Assert.That(firstRepository.LoadBy(p => p.Id == peter.Id).LastName, Is.EqualTo(peter.LastName));
                Assert.That(() => firstRepository.LoadBy(p => p.Id == golo.Id), Throws.Exception);

                firstRepository.SaveOnSubmit(golo);
            }

            var secondRepository = XmlRepository.GetInstance<Person>();
            Assert.That(secondRepository.LoadAll().Count(), Is.EqualTo(2));
            Assert.That(secondRepository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
            Assert.That(secondRepository.LoadAllBy(p => p.Id == peter.Id).Count(), Is.EqualTo(1));
            Assert.That(secondRepository.LoadAllBy(p => p.Id == golo.Id).Count(), Is.EqualTo(1));
            Assert.That(() => secondRepository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);
            Assert.That(secondRepository.LoadBy(p => p.Id == peter.Id).LastName, Is.EqualTo(peter.LastName));
            Assert.That(secondRepository.LoadBy(p => p.Id == golo.Id).LastName, Is.EqualTo(golo.LastName));
        }
    }
}