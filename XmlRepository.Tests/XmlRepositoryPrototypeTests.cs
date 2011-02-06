using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;
using XmlRepository.DataProviders;
using XmlRepository.Mapping;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests
{
    [TestFixture]
    public class XmlRepositoryPrototypeTests
    {
        [Test]
        public void SupportOneToManyAndManyToOneThroughtInteractionOfMultipleRepositoriesWithinARepository()
        {
            var articleInBothCategories = new Article {Title = "Lambda auf Abwegen"};

            var articlesOne = new List<Article>
                               {
                                  articleInBothCategories,
                                   new Article {Title = "Frs Gemht"},
                                   new Article {Title = "Schlechter aber rechter"}
                               };

            var articlesTwo = new List<Article>
                               {
                                   new Article {Title = "Test Artikel"},
                                   articleInBothCategories,
                                   new Article {Title = "Kren fllen"}
                               };

            var categoryOne = new ArticleCategory {Name = "One"};
            var categoryTwo = new ArticleCategory {Name = "Two"};

            string content = null;

            using(var repository = XmlRepository.Get(RepositoryFor<ArticleCategory>
                .WithIdentity(c => c.Id)
                .WithDataProvider(new FileDataProvider(Environment.CurrentDirectory, ".xml"))))
            {
                categoryOne.Articles = articlesOne;

                repository.SaveOnSubmit(categoryOne);
                repository.SubmitChanges();

                var data = repository.LoadAll();
            }
        }

        [Test]
        public void RepositoryCanBeInstantiatedOnAllWaysWithoutReturningNullOrExceptionThrown()
        {
            var one = XmlRepository.Get(
                RepositoryFor<Person>.WithIdentity(p => p.Id));

            var two = XmlRepository.Get(RepositoryFor<Person>
                                            .WithIdentity(p => p.Id)
                                            .WithMappings(new Dictionary<Type, IList<IPropertyMapping>>()));

            var three = XmlRepository.Get(RepositoryFor<Person>
                                              .WithIdentity(p => p.Id)
                                              .WithDataProvider(new InMemoryDataProvider()));

            var four = XmlRepository.Get(RepositoryFor<Person>
                                             .WithIdentity(p => p.Id)
                                             .WithMappings(new Dictionary<Type, IList<IPropertyMapping>>())
                                             .WithDataProvider(new InMemoryDataProvider()));

            Assert.That(one, Is.Not.Null);
            Assert.That(two, Is.Not.Null);
            Assert.That(three, Is.Not.Null);
            Assert.That(four, Is.Not.Null);
        }

        [Test]
        public void MappingBuilderAbstractionIntegration()
        {
            using (var builder = XmlRepository.GetPropertyMappingBuilderFor<Person>())
            {
                builder.Map(p => p.Id).ToAttribute("sexy");
                builder.Map(p => p.LastName).ToContent();
            }

            //// builder.Map(p => p.Geek).ToElement("TESTGEEK"); ??

            string xml = "<root></root>";

            var delegateProvider = new DelegateDataProvider(() => xml, data => xml = data);
            XmlRepository.DataProvider = delegateProvider;

            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                var person = new Person
                                 {
                                     FirstName = "Peter&",
                                     LastName = "Bucher",
                                     Geek = new Geek { Alias = "YeahAlias", SuperId = "test" }
                                 };

                repository.SaveOnSubmit(person);
                repository.SubmitChanges();

                var data = repository.LoadAll();
            }
        }

        [Test]
        public void DataAvailableInProviderIsLoadAutomatically()
        {
            var getRepositoryThatWillBeReused = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id));

            var builder = XmlRepository.GetPropertyMappingBuilderFor<Person>();

            builder.Map(p => p.Id).ToAttribute();

            builder.ApplyMappingsGlobal();

            var prefilledInMemoryProvider = new InMemoryDataProvider(
                @"<root>
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
</root>");

            using (var repository = XmlRepository.Get(RepositoryFor<Person>
                .WithIdentity(p => p.Id)
                .WithDataProvider(prefilledInMemoryProvider)))
            {
                var test = repository.LoadAll();

                Assert.That(repository.LoadAll(), Is.Not.Null);
            }
        }

        [Test]
        public void PropertyMappingPrototypeTest()
        {
            var mapping = new PropertyMapping();
            mapping.EntityType = typeof(Person);
            mapping.PropertyType = typeof(Guid);
            mapping.Name = "Id";
            mapping.XmlMappingType = XmlMappingType.Attribute;

            XmlRepository.AddPropertyMappingFor(typeof(Person), mapping);

            var test = new PropertyMapping();
            test.EntityType = typeof(Person);
            test.PropertyType = typeof(string);
            test.Name = "LastName";
            test.XmlMappingType = XmlMappingType.Element;

            XmlRepository.AddPropertyMappingFor(typeof(Person), test);

            var a = new PropertyMapping();
            a.EntityType = typeof(Geek);
            a.PropertyType = typeof(string);
            a.Name = "SuperId";
            a.XmlMappingType = XmlMappingType.Attribute;

            XmlRepository.AddPropertyMappingFor(typeof(Geek), a);

            using (var repository = XmlRepository
                .Get(RepositoryFor<Person>
                .WithIdentity(p => p.Id)
                .WithDataProvider(new InMemoryDataProvider())))
            {
                var geek = new Geek { Alias = "Jackal" };

                var peter = new Person
                                {
                                    FirstName = "Peter",
                                    LastName = "Bucher",
                                    Birthday = new DateTime(1983, 10, 17),
                                    Friends = new List<Geek>(new[] { geek, new Geek() { Alias = "YEAH" } }),
                                    Geek = geek
                                };

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
            using (var repository = XmlRepository.Get(RepositoryFor<Person>
                .WithIdentity(p => p.Id)
                .WithDataProvider(new InMemoryDataProvider())))
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