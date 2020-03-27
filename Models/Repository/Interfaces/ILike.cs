using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface ILike
    {
        IEnumerable<Like> GetAll();
        Like GetById(int Id);
        void Insert(Like like);
        void Update(Like like);
        void Delete(int Id);
    }
}
