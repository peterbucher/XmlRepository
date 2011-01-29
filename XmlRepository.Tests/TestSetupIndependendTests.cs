using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XmlRepository.DataMapper;
using XmlRepository.DataProviders;
using XmlRepository.DataSerializers;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests
{
    [TestFixture]
    public class TestSetupIndependendTests
    {
        [SetUp]
        public void InitializeDataProvider()
        {
            XmlRepository.DataMapper = new ReflectionDataMapper();
            XmlRepository.DataSerializer = new XmlDataSerializer();
            XmlRepository.DataProvider = new InMemoryDataProvider();
            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                repository.DeleteAllOnSubmit();
            }
        }

        [Test]
        public void TestPlayField()
        {
            string data = @"<root>
  <Person>
    <Id>c186fb12-dd38-4784-8b48-aad96a6510c4</Id>
    <LastName>Bucher</LastName>
    <FirstName>Peter</FirstName>
    <Geek>
      <Id>8f8b747e-3f16-4938-a384-980fc8aa8dd7</Id>
      <SuperId>test</SuperId>
      <Alias>Jackal</Alias>
    </Geek>
    <Friends></Friends>
    <Birthday>17.10.1983 00:00:00</Birthday>
  </Person>
</root>";

            XmlRepository.DataProvider = new DelegateDataProvider(() => data, xml => data = xml);

            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                repository.DiscardChanges();

                var test = repository.LoadAll().Single();

                test.FirstName = "Hanswurscht";

                repository.SaveOnSubmit(test);
                repository.SubmitChanges();

                repository.SaveOnSubmit(new Person() { FirstName = "Peter" });
                repository.SaveOnSubmit(new Person() { FirstName = "Golo" });

                var peter = repository.LoadBy(p => p.FirstName == "Peter");

                repository.DeleteOnSubmit(p => p.FirstName == "Peter");

                var test2 = repository.LoadAll();
            }
        }

        [Test]
        public void PropertyMappingPrototypeTest()
        {
            XmlRepository.DataProvider = new InMemoryDataProvider();

            var mapping = new PropertyMapping();
            mapping.EntityType = typeof(Person);
            mapping.PropertyType = typeof(Guid);
            mapping.Name = "Id";
            mapping.MapType = MapType.Attribute;

            XmlRepository.AddMappingFor(typeof(Person), mapping);

            var test = new PropertyMapping();
            test.EntityType = typeof(Person);
            test.PropertyType = typeof(string);
            test.Name = "LastName";
            test.MapType = MapType.Element;

            XmlRepository.AddMappingFor(typeof(Person), test);

            var a = new PropertyMapping();
            a.EntityType = typeof(Geek);
            a.PropertyType = typeof(string);
            a.Name = "SuperId";
            a.MapType = MapType.Attribute;

            XmlRepository.AddMappingFor(typeof(Geek), a);

            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                var geek = new Geek { Alias = "Jackal" };

                var peter = new Person();

                peter.FirstName = "Peter";
                peter.LastName = "Bucher";
                peter.Birthday = new DateTime(1983, 10, 17);
                peter.Friends = new List<Geek>(new[] { geek, new Geek() { Alias = "YEAH" } });

                peter.Geek = geek;

                repository.SaveOnSubmit(peter);

                var loadedData = repository.LoadAll();

                Assert.That(loadedData.Count(), Is.EqualTo(1));
                Assert.That(loadedData.First().FirstName == "Peter");
                Assert.That(loadedData.First().Friends.Count, Is.EqualTo(2));
            }
        }



        [Test]
        public void Foo()
        {
            XmlRepository.DataProvider = new InMemoryDataProvider();

            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                repository.SaveOnSubmit(new Person() { FirstName = "Peter" });
                repository.SaveOnSubmit(new Person() { FirstName = "Golo" });

                var test = repository.LoadAll();
                var peter = repository.LoadBy(p => p.FirstName == "Peter");

                repository.DeleteOnSubmit(p => p.FirstName == "Peter");

                var test2 = repository.LoadAll();
            }
        }
    }
}