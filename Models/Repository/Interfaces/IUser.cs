using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface IUser
    {
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(int Id);
        void Insert(ApplicationUser applicationUser);
        void Update(ApplicationUser applicationUser);
        void Delete(int Id);
    }
}
