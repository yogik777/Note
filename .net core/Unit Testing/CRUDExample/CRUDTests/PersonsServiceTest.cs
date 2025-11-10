using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using System.Security.Cryptography.X509Certificates;
using Xunit.Abstractions;
using Entities;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        //private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;


            //var countriesInitailData = new List<Country>() { };
            //var personInitialData = new List<Person>() { };


            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitailData);
            //dbContextMock.CreateDbSetMock(temp => temp.Persons, personInitialData);

            //_countriesService = new CountriesService(null);

            
            _personsService = new PersonsService(_personsRepository);
            
            _testOutputHelper = testOutputHelper;
        }

        #region Add Person
        //When you supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            ////Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            //    //Act 
            //    await _personsService.AddPerson(personAddRequest);
            //});


            Func<Task> action = async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        //When we supply null value as PersonName, it should throw ArgumentException
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "anyone@email.com")
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            //When PersonsRepository.AddPerson is called it has to return the same "person" object
            _personsRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            ////Assert 
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{   
            //    //Act 
            //    await _personsService.AddPerson(personAddRequest);
            //});

            Func<Task> action = async () =>
            {
                //Act
                await _personsService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonRespone, which includes with the newly generated person id
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //If we supply any argument value to the AddPerson method, it should return the same return value
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act 
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            //Assert 
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

        }
        #endregion


        #region GetPersonByPersonID 
        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange 
            Guid? personID = null;

            //Act 
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

            //Assert 
            //Assert.Null(person_response_from_get);

            person_response_from_get.Should().BeNull();
        }

        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSucessful()
        {
            //Arrange 
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone@email.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();


            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(person.PersonID))
                .ReturnsAsync(person);
            //Act
            PersonResponse person_respones_from_get = await _personsService.GetPersonByPersonID(person.PersonID);


            //Assert 
            //Assert.Equal(person_response_from_add, person_respones_from_get);
            person_respones_from_get.Should().Be(person_response_expected);
        }
        #endregion

        #region GetAllPersons
        [Fact]
        //The GetAllPersons() should return an empty list by default 
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personsRepositoryMock
                .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //Act 
            List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

            //Assert
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange 

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone1@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone2@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone3@email.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();



            //print person_response_list_from_add 
            _testOutputHelper.WriteLine("Expected: ");
            foreach( PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);


            //Act 
            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

            //print person_response_list_from_get_All 
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_get.ToString());
            }

            //Assert 
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_get);

            //}
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons
        //If the search text is empty and search by is "PersonName", it should return all persons 
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone1@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone2@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone3@email.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add 
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);
                

            //Act 
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName),"");


            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        //First we will add few persons; and then we will search based on the person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessul()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone1@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone2@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone3@email.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add 
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);


            //Act 
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "a");


            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending order on PersonName 
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange 
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone1@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone2@email.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone3@email.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            //List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock
                .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            //Act 
            List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);


            //Assert 
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            //}

            //persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException 
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange 
            PersonUpdateRequest? person_update_request = null;

            //Assert 
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //   //Act 
            //   await _personsService.UpdatePerson(person_update_request);
            //});

            Func<Task> action = async () =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        //When we supply invalid person id, it should throw ArgumentException 
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange 
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
                .Create();

            //Act 
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            };

            //Assert 
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //When the PersonName is null, it should throw ArgumentException  
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange 
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email , "anyone@email.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            //Act
            var action = async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
            
        }

        //First, add a new person and try to update the person name and email  
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {


            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "anyone@email.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personsRepositoryMock
                .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

            //Assert 
            person_response_from_update.Should().Be(person_response_expected);

        }
        #endregion

        #region DeletePerson

        //If you supply a valid PersonID, it should return true 
        [Fact]
        public async Task DeletePerson_validPersonID()
        {
            //Arrange 
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "anyone@email.com")
                .With(temp => temp.Gender, "Female")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();
            _personsRepositoryMock
                .Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _personsRepositoryMock
                .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act 
            bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

            //Assert 
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid PersonID, it should return false 
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {

            //Act 
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert 
            Assert.False(isDeleted);
        }
        #endregion
    }
}
