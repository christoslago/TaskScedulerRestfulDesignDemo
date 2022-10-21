using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class AzureParams
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string Domain { get; set; }
        public string Instance { get; set; }
        public string ClientSecret { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string Scopes { get; set; }
    }
}
