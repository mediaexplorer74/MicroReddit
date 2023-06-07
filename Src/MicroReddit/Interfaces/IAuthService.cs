using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroReddit.Entities;

namespace MicroReddit.Interfaces
{
    public interface IAuthService
    {
        Task<AuthInfo> GetAuthInfo(string username, string password, string client_id, string client_secret);
    }
}
