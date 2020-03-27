using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface IReplay
    {
        IEnumerable<Replay> GetAll();
        Replay GetById(int Id);
        void Insert(Replay replay);
        void Update(Replay replay);
        void Delete(int Id);
    }
}
