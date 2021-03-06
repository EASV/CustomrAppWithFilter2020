using System.Collections.Generic;
using CustomerApp.Core.Entity;

namespace CustomerApp.Core.ApplicationService
{
    public interface IAddressService
    {
        Address Create(Address address);
        
        List<Address> GetAll();

        Address GetById(int id);
        Address Update(Address address);
    }
}