using System;
using System.Collections.Generic;
using System.Linq;
using CustomerApp.Core.ApplicationService.Exceptions;
using CustomerApp.Core.DomainService;
using CustomerApp.Core.Entity;

namespace CustomerApp.Core.ApplicationService.Services
{
    public class CityService: ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly ICityValidator _cityValidator;
        
        public CityService(
            ICityValidator cityValidator,
            ICityRepository cityRepository)
        {
            _cityValidator = cityValidator ?? throw new NullReferenceException("Validator Cannot be null");
            _cityRepository = cityRepository ?? throw new NullReferenceException("CityRepository Cannot be Null");
        }
        
        public FilteredList<City> ReadAll(Filter filter = null)
        {
            if (filter == null)
            {
                filter = new Filter(){CurrentPage = 1, ItemsPrPage = 5};
            }
            return _cityRepository.GetAll(filter);
        }

        public City Create(City city)
        {
            _cityValidator.DefaultValidation(city);
            return _cityRepository.Create(city);
        }

        public City FindCityByZipcode(int zipCode)
        {
            return _cityRepository.ReadById(zipCode);
        }

        public City Delete(int zipCode)
        {
                var city = _cityRepository.ReadById(zipCode);
                if (city == null)
                {
                    throw new Exception("City not found.. WRAAAAHHHHHHH!!!! ");
                }
                return _cityRepository.Delete(zipCode);
            
        }

        public City Update(City city)
        {
            try
            {
                _cityValidator.DefaultValidation(city);
                return _cityRepository.Update(city);
            }
            catch (DataSourceException ds)
            {
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}