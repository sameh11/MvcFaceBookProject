using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface IFriendRequest
    {
        IEnumerable<FriendRequest> GetAll();
        FriendRequest GetById(int Id);
        void Insert(FriendRequest friendRequest);
        void Update(FriendRequest friendRequest);
        void Delete(int Id);
    }
}
