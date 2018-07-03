using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using System.Configuration;

namespace ConsoleApplication1
{
    class Program
    {
        private static object sec;


        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {

            var retrievedSecret = await GetKey();
            Console.WriteLine("Secret retrieved successfully.");
            Console.WriteLine("Secret is : "+ retrievedSecret);
            Console.WriteLine(" ");
            Console.WriteLine("Press ANY key to end program...");
            Console.ReadLine();
        }
        
        private static async Task<string> GetKey()
        {

            var client = new KeyVaultClient(
            new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync),
            new System.Net.Http.HttpClient());

            Console.WriteLine("Please enter your vault URL (for example, https://myKeyVault.vault.azure.net) :");
            var vaultUrl = Console.ReadLine();
            Console.WriteLine("Please enter the name of your Secret (for example, MySQLConnectionString) :");
            var secretUrl = Console.ReadLine();


            var secret = Task.Run(() => client.GetSecretAsync(vaultUrl, secretUrl)).Result;

            Console.WriteLine("Initiating new connection using :");
            Console.WriteLine("Vault URI : "+ vaultUrl.ToString() + " and secret : " +secretUrl.ToString());
            Console.WriteLine("Press ANY key to continue...");
            Console.ReadLine();

            return secret.Value;

        }
        
        private static async Task<string> GetAccessTokenAsync(
       string authority,
       string resource,
       string scope)
        {

            // The ClientId = the Application ID of the registered Enterprise Application
            // Azure Active Directory >> Enterprise Apps >>
            // <your app> >> Properties
            Console.WriteLine("Please enter the Application ID of the App (for example, 0103xxxx-xxxx-xxxx-xxxx-xxxxxxxx5888");
            //var clientId = ConfigurationManager.AppSettings["clientId"];
            var clientId = Console.ReadLine();

            // clientSecret = secret key generated in Enterprise Apps at creation time
            // Ref : https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#get-application-id-and-authentication-key

            Console.WriteLine("Please enter the Secret of the App (for example, zXYDFlGcwK1Fc+NxxxxxxxxxxxxtSbetGWxxxxx5s5A=");
            var clientSecret = Console.ReadLine();
            //var clientSecret = ConfigurationManager.AppSettings["clientSecret"];

            Console.WriteLine("Retrieving a secret from Azure Key Vault using the client credentials specified.");
            Console.WriteLine("Press ANY key to continue...");
            Console.ReadLine();

            var clientCredential = new ClientCredential(
                clientId,
                clientSecret);

            var context = new AuthenticationContext(
                authority,
                TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(
                resource,
                clientCredential);

            return result.AccessToken;
        }


    }
}
