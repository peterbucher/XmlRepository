using System;
using NUnit.Framework;
using XmlRepository.DataMapper;
using XmlRepository.DataProviders;
using XmlRepository.Tests.Entities;

namespace XmlRepository.Tests
{
    [TestFixture]
    public class IndependendTests
    {
        [Test]
        public void FoooooBar()
        {
            XmlRepository.DataProvider = new FileDataProvider(Environment.CurrentDirectory, "xml");

            var mapping = new PropertyMapping();
            mapping.EntityType = typeof (Person);
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
            a.EntityType = typeof (Geek);
            a.PropertyType = typeof(string);
            a.Name = "SuperId";
            a.MapType = MapType.Attribute;

            XmlRepository.AddMappingFor(typeof(Geek), a);

            using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
            {
                var peter = new Person();
                peter.FirstName = "Peter";
                peter.LastName = "Bucher";
                peter.Birthday = new DateTime(1983, 10, 17);

                peter.Geek = new Geek { Alias = "Jackal"};

                repository.SaveOnSubmit(peter);

                var loadedData = repository.LoadAll();
            }
        }

        //[Test]
        //public void Test()
        //{
        //    string data = @"<root><Person><Id>{76447A86-17DD-41A0-90D1-A3F658D4B238}</Id><FirstName>Sepp</FirstName></Person></root>";

        //    XmlRepository.DataProvider = new DelegateDataProvider(() => data, xml => data = xml);

        //    using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
        //    {
        //        var test = repository.LoadAll().First();

        //        test.FirstName = "Hanswurscht";

        //        repository.SaveOnSubmit(test);
        //        repository.SubmitChanges();

        //        repository.SaveOnSubmit(new Person() { FirstName = "Peter" });
        //        repository.SaveOnSubmit(new Person() { FirstName = "Golo" });

        //        var peter = repository.LoadBy(p => p.FirstName == "Peter");

        //        repository.DeleteOnSubmit(p => p.FirstName == "Peter");

        //        var test2 = repository.LoadAll();
        //    }
        //}

        //[Test]
        //public void Foo()
        //{
        //    XmlRepository.DataProvider = new InMemoryDataProvider();

        //    using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
        //    {
        //        repository.SaveOnSubmit(new Person() { FirstName = "Peter" });
        //        repository.SaveOnSubmit(new Person() { FirstName = "Golo" });

        //        var test = repository.LoadAll();
        //        var peter = repository.LoadBy(p => p.FirstName == "Peter");

        //        repository.DeleteOnSubmit(p => p.FirstName == "Peter");

        //        var test2 = repository.LoadAll();
        //    }
        //}
    }

    //[TestFixture]
    //public class XmlRepositoryTests
    //{
    //    private Person _peter;
    //    private Person _golo;

    //    [SetUp]
    //    public void InitializeEntities()
    //    {
    //        this._peter = new Person
    //                          {
    //                              Id = Guid.NewGuid(),
    //                              FirstName = "Peter",
    //                              LastName = "Bucher",
    //                              Birthday = new DateTime(1983, 10, 17)
    //                          };
    //        this._golo = new Person
    //                         {
    //                             Id = Guid.NewGuid(),
    //                             FirstName = "Golo",
    //                             LastName = "Roden",
    //                             Birthday = new DateTime(1978, 9, 27)
    //                         };
    //    }

    //    [TearDown]
    //    public void DestroyEntities()
    //    {
    //        this._peter = null;
    //        this._golo = null;
    //    }

    //    [SetUp]
    //    public void InitializeDataProvider()
    //    {
    //        XmlRepository.DataMapper = new ReflectionDataMapper();
    //        XmlRepository.DataSerializer = new XmlDataSerializer();
    //        XmlRepository.DataProvider = new InMemoryDataProvider();
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.DeleteAllOnSubmit();
    //        }
    //    }

    //    [TearDown]
    //    public void DestroyDataProvider()
    //    {
    //        XmlRepository.DataMapper = null;
    //        XmlRepository.DataSerializer = null;
    //        XmlRepository.DataProvider = null;
    //    }

    //    [Test]
    //    public void ARepositoryCanLoadGeekFriendCollectionFromPerson()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            var geekFriends = new List<Geek> { new Geek { Alias = "Peter" }, new Geek() { Alias = "Golo" } };
    //            var personWithGeekFriends = new Person() { FirstName = "Roberto", Friends = geekFriends };

    //            repository.SaveOnSubmit(personWithGeekFriends);

    //            var all = repository.LoadAll();

    //            var robertoWithGeeks = repository.LoadBy(p => p.FirstName == "Roberto");
    //            var robertosFriends = robertoWithGeeks.Friends;

    //            var test = repository.LoadBy(p => p.Friends != null && p.Friends.Count > 0);
    //        }
    //    }

    //    [Test]
    //    public void ANewlyCreatedRepositoryWithNoDefaultQueryPropertySetInferesItFromTheLambdaExpression()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Geek>.WithIdentity(g => g.SuperId)))
    //        {
    //            repository.SaveOnSubmit(new Geek { SuperId = "PeterId" });
    //            repository.SubmitChanges();

    //            var test = repository.LoadBy("PeterId");

    //            Assert.That(test.SuperId, Is.EqualTo("PeterId"));
    //        }
    //    }

    //    [Test]
    //    public void ANewlyCreatedRepositoryDoesNotContainAnyEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            this.ExecuteLoadAsserts(repository, 0, false, false);
    //        }
    //    }

    //    [Test]
    //    public void SaveOnSubmitAnEntitySavesTheEntity()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 1, true, false);
    //    }

    //    [Test]
    //    public void SaveOnSubmitUpdatesAnEntity()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 1, true, false);
    //    }

    //    [Test]
    //    public void SaveOnSubmitMultipleEntitiesSavesTheEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 2, true, true);
    //    }

    //    [Test]
    //    public void SaveOnSubmitNullThrowsArgumentNullException()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            Assert.Throws<ArgumentNullException>(() => repository.SaveOnSubmit((Person)null));
    //            Assert.Throws<ArgumentNullException>(() => repository.SaveOnSubmit((IEnumerable<Person>)null));
    //        }
    //    }

    //    [Test]
    //    public void DiscardChangesRemovesNonSubmittedChanges()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //            repository.SubmitChanges();
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //            repository.SaveOnSubmit(this._golo);
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //            repository.DiscardChanges();
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 1, true, false);
    //    }

    //    [Test]
    //    public void DeleteAllRemovesAllEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //            repository.DeleteAllOnSubmit();
    //            this.ExecuteLoadAsserts(repository, 0, false, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 0, false, false);
    //    }

    //    [Test]
    //    public void DeleteOnSubmitNullThrowsArgumentNullException()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            Assert.Throws<ArgumentNullException>(() => repository.DeleteOnSubmit(null));
    //        }
    //    }

    //    [Test]
    //    public void DeleteOnSubmitWithLambdaRemovesMatchingEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //            repository.DeleteOnSubmit(p => p.LastName == this._golo.LastName);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 1, true, false);
    //    }

    //    [Test]
    //    public void DeleteOnSubmitWithIdentityRemovesMatchingEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //            repository.DeleteOnSubmit(this._golo.Id);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);
    //        }
    //        this.ExecuteLoadAsserts(XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)), 1, true, false);
    //    }

    //    [Test]
    //    public void DeleteOnSubmitWithIdentityThrowsExceptionWhenNoEntityWasFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);

    //            Assert.Throws<EntityNotFoundException>(() => repository.LoadBy(this._golo.Id));
    //        }
    //    }

    //    [Test]
    //    public void LoadAllReturnsAllEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            var entities = repository.LoadAll();
    //            Assert.That(entities.Count(), Is.EqualTo(2));
    //        }
    //    }

    //    [Test]
    //    public void LoadAllByNullThrowsArgumentNullException()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            Assert.Throws<ArgumentNullException>(() => repository.LoadAllBy(null));
    //        }
    //    }

    //    [Test]
    //    public void LoadAllByReturnsEmptyCollectionWhenNoMatchingEntitiesWereFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(this._peter);
    //            this.ExecuteLoadAsserts(repository, 1, true, false);

    //            var entities = repository.LoadAllBy(p => p.LastName == this._golo.LastName);
    //            Assert.That(entities.Count(), Is.EqualTo(0));
    //        }
    //    }

    //    [Test]
    //    public void LoadAllByReturnsSingleMatchingEntityWhenOnlyOneEntityIsFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            var singleEntity = repository.LoadAllBy(p => p.LastName == this._golo.LastName);
    //            Assert.That(singleEntity.Count(), Is.EqualTo(1));
    //        }
    //    }

    //    [Test]
    //    public void LoadAllByReturnsMultipleMatchingEntitiesWhenMoreThanOneEntityWasFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            var multipleEntities = repository.LoadAllBy(p => true);
    //            Assert.That(multipleEntities.Count(), Is.EqualTo(2));
    //        }
    //    }

    //    [Test]
    //    public void LoadByNullThrowsArgumentNullException()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            Assert.Throws<ArgumentNullException>(() => repository.LoadBy(null));
    //        }
    //    }

    //    [Test]
    //    public void LoadByLambdaThrowsExceptionWhenNoEntitiesWereFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            Assert.Throws<EntityNotFoundException>(() => repository.LoadBy(p => p.Id == Guid.Empty));
    //        }
    //    }

    //    [Test]
    //    public void LoadByLambdaReturnsSingleMatchingEntityWhenOnlyOneEntityWasFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            var entities = repository.LoadBy(p => p.LastName == this._golo.LastName);
    //            Assert.That(entities, Is.Not.Null);
    //        }
    //    }

    //    [Test]
    //    public void LoadByLambdaThrowsExceptionWhenMoreThanOneEntityWasFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            Assert.Throws<InvalidOperationException>(() => repository.LoadBy(p => true));
    //        }
    //    }

    //    [Test]
    //    public void LoadByIdentityThrowsExceptionWhenNoEntitiesWereFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            Assert.Throws<EntityNotFoundException>(() => repository.LoadBy(Guid.Empty));
    //        }
    //    }

    //    [Test]
    //    public void LoadByIdentityReturnsSingleMatchingEntityWhenOnlyOneEntityWasFound()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            var entities = repository.LoadBy(this._golo.Id);
    //            Assert.That(entities, Is.Not.Null);
    //        }
    //    }

    //    [Test]
    //    public void GetEnumeratorReturnsAnEnumerator()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);

    //            int count = 0;
    //            foreach (var person in repository)
    //            {
    //                Assert.That(person.Id, Is.Not.EqualTo(Guid.Empty));
    //                count++;
    //            }
    //            Assert.That(count, Is.EqualTo(2));
    //        }
    //    }

    //    [Test]
    //    public void RunningLinqQueriesReturnsAllMatchingEntities()
    //    {
    //        using (var repository = XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id)))
    //        {
    //            repository.SaveOnSubmit(new[] { this._peter, this._golo });
    //            this.ExecuteLoadAsserts(repository, 2, true, true);
    //        }

    //        var firstName =
    //            (from p in XmlRepository.Get(RepositoryFor<Person>.WithIdentity(p => p.Id))
    //             where p.LastName == this._peter.LastName
    //             select p.FirstName).Single();

    //        Assert.That(firstName, Is.EqualTo(this._peter.FirstName));
    //    }

    //    private void ExecuteLoadAsserts(
    //        IXmlRepository<Person, Guid> repository,
    //        int totalNumberOfEntities,
    //        bool isPeterContained,
    //        bool isGoloContained)
    //    {
    //        Assert.That(repository.LoadAll().Count(), Is.EqualTo(totalNumberOfEntities));

    //        Assert.That(repository.LoadAllBy(p => p.Id == Guid.Empty).Count(), Is.EqualTo(0));
    //        Assert.That(() => repository.LoadBy(p => p.Id == Guid.Empty), Throws.Exception);

    //        Assert.That(repository.LoadAllBy(p => p.Id == this._peter.Id).Count(), Is.EqualTo(isPeterContained ? 1 : 0));
    //        if (isPeterContained)
    //            Assert.That(repository.LoadBy(p => p.Id == this._peter.Id).LastName, Is.EqualTo(this._peter.LastName));
    //        else
    //            Assert.That(() => repository.LoadBy(p => p.Id == this._peter.Id), Throws.Exception);

    //        Assert.That(repository.LoadAllBy(p => p.Id == this._golo.Id).Count(), Is.EqualTo(isGoloContained ? 1 : 0));
    //        if (isGoloContained)
    //            Assert.That(repository.LoadBy(p => p.Id == this._golo.Id).LastName, Is.EqualTo(this._golo.LastName));
    //        else
    //            Assert.That(() => repository.LoadBy(p => p.Id == this._golo.Id), Throws.Exception);
    //    }
    //}
}