using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface IPost
    {
        IEnumerable<Post> GetAll();
        Post GetById(int Id);
        void Insert(Post post);
        void Update(Post post);
        void Delete(int Id);
    }
}
