using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroReddit.Entities;

namespace MicroReddit.Interfaces
{
    public interface IRedditService
    {
        Task<List<Post>> GetTopPostsByLimit(int limit);
    }
}
