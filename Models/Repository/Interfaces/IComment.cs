using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Interfaces
{
    public interface IComment
    {
        IEnumerable<Comment> GetAll();
        Comment GetById(int Id);
        void Insert(Comment comment);
        void Update(Comment comment);
        void Delete(int Id);
    }
}
