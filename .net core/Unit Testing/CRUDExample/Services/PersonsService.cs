using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;


namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private field 
        //private readonly ApplicationDbContext _db;
        private readonly IPersonsRepository _personsRepository;
        //private readonly ICountriesService _countriesService;


        //constructor 
        public PersonsService(IPersonsRepository personsRepository)
        {
            //_db = personsDbContext;
            _personsRepository = personsRepository;
            //_countriesService = countriesService;

        }

        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    personResponse.Country = person.Country?.CountryName;
        //    return personResponse;
        //}

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if "personAddRequest" is not null
            if(personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }
            //Validate all properties of "personAddRequest"
            //Validate PersonName 
            //if (string.IsNullOrEmpty(personAddRequest.PersonName))
            //{
            //    throw new ArgumentException("PersonName can't be blank");
            //}

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);


            //Convert "personAddRequest" from "PersonAddRequest" type to "Person"
            Person person = personAddRequest.ToPerson();

            //Generate a new PersonID 
            person.PersonID = Guid.NewGuid();

            //Then add it into List<Person>
            await _personsRepository.AddPerson(person);
            //_db.sp_InsertPerson(person);

            //Return PersonResponse object with generated PersonID 
            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //Convert all persons from "Person" type to "PersonResponse" type.
            //Return all PersonResponse objects 
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(temp =>  temp.ToPersonResponse()).ToList();

            //return _db.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            //Check if "personID" is not null.
            if (personID == null)
                return null;

            //Get matching person from List<Person> based personID.
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
                return null;


            //Convert matching person object from "Person" to "PersonResponse" type.
            //Return PersonResponse object 
            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            //Check if "searchBy" is not null.
            //Get matching persons from List<Person> based on given searchBy and searchString.
            //Convert the matching persons from "Person" type to "PersonResponse" type.
            //Return all matching PersonResponse objects 
            //List<PersonResponse> allPersones = await GetAllPersons();
            //List<PersonResponse> matchingPersons = allPersones;

            //if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            //    return matchingPersons;

            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                                temp.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                                temp.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                                temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                                temp.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryID) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                                temp.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                                temp.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
                switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),
                _ => allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //Check if "personUpdateRequest" is not null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //Validate all properties of "perosnUpdateRequest"
            ValidationHelper.ModelValidation(personUpdateRequest);

            //Get the matching "Person" object from List<Person> based on PersonID.
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);


            //Check if matching "Person" object is not null
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //Update all details from "PersonUpdateRequest" object to "Person" object 
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);

            //Convert the person object from "Person" to "PersonResponse" type 
            //Return PersonResponse object with updated details \

            return matchingPerson.ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            //Check if "personID" is not null
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            //Get the matching "Person" object from List<Person> based on PersonID.
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            //Check if matching "Person" object is not null
            if (person == null)
                return false;

            //Delete the matching "Person" object from List<Person>
            //Return Boolean value indicating whether person object was deleted or not 
            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;

        }
    }
}
