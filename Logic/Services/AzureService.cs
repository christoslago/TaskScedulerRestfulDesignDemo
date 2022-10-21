using Azure.Identity;
using Logic.Helpers;
using Logic.Services.Interfaces;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Formats.Asn1.AsnWriter;

namespace Logic.Services
{
    public class AzureService:IAzureService
    {
        private AzureParams AzureParameters;
        public AzureService(AzureParams azureParams)
        {
            AzureParameters = azureParams;
        }
        public async Task<List<User>> GetAzureUsers()
        {
            var usersList = new List<User>();
            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var clientSecretCredential = new ClientSecretCredential(AzureParameters.TenantId, AzureParameters.ClientId, AzureParameters.ClientSecret);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
                var users = await graphClient.Users
                    .Request()
                    .Select(e => new
                    {
                        e.DisplayName,
                        e.Id,
                        e.Identities,
                        e.Mail,
                        e.GivenName,
                        e.Surname,
                        e.UserPrincipalName
                    })
                    .GetAsync();

                foreach(var usr in users)
                {
                    usersList.Add(usr);
                }

                return usersList;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }



        public async Task<AuthenticationResult> GetATokenForGraph(string username, string password)
        {
            string[] scopes = new string[] { "https://christoslagossampleapi.onmicrosoft.com/GraphAPI/User.ReadWrite.All", "https://christoslagossampleapi.onmicrosoft.com/GraphAPI/User.Read" };
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create("e677c9e3-e547-463d-96ab-c08e7d5217c8")
                                              .WithB2CAuthority($"{AzureParameters.Instance}/tfp/christoslagossampleapi.onmicrosoft.com/{AzureParameters.SignUpSignInPolicyId}")
                                              .Build();
            var accounts = await app.GetAccountsAsync("B2C_1_simpleusersignin");

            AuthenticationResult result = null;
            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                try
                {
                    var securePassword = new SecureString();
                    foreach (char c in password)
                        securePassword.AppendChar(c);

                    result = await app.AcquireTokenByUsernamePassword(scopes, username, securePassword).ExecuteAsync();
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }
        

    }
}
