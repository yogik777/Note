using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesService : ICountriesService
	{
		//private field 
		//private readonly ApplicationDbContext _db;
		private readonly ICountriesRepository _countriesRepository;


		//constructor 
		public CountriesService(ICountriesRepository countriesRepository)
		{
			//_db = personsDbContext;
			_countriesRepository = countriesRepository;
        }


		public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
		{

            //Validation: countryAddRequest parameter can't be null 
            if (countryAddRequest == null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest));
			}

			//Validation: countryName can't be null 
			if(countryAddRequest.CountryName == null)
			{
				throw new ArgumentException(nameof(countryAddRequest.CountryName));
			}

			//Validation: CountryName can't be duplicate 
			if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
			{
				throw new ArgumentException("Given Country name already exists");
			}

			//Convert object from CountryAddRequest to Country type 
			Country country = countryAddRequest.ToCountry();

			//generate CountryID
			country.CountryID = Guid.NewGuid();

			//Add country object into _countries 
			await _countriesRepository.AddCountry(country);
			return country.ToCountryResponse();

		}

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            //Convert all countries from "Country" type to "CountryResponse" type. 
            //Return all CountryResponse objects 
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
			//Check if "countryID" is not null
			if (countryID == null)
				return null;

			//Get matching country from List<Country> based countryID.
			Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

			if (country_response_from_list == null)
				return null;

			//Convert matching country object from "Country" to "CountryResponse" type
			//Return CountryResponse object 
			return country_response_from_list.ToCountryResponse();

        }
    }
}
