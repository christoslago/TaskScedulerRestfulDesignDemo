using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services.Interfaces
{
    public interface IAzureService
    {
        Task<List<User>> GetAzureUsers();
        Task<AuthenticationResult> GetATokenForGraph(string username, string password);
    }
}
