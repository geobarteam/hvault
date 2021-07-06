using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace SendRabbitMq
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // use VaultSharp see doc: https://github.com/rajanadar/VaultSharp
            // configure vault client
            var vaultServerUriWithPort = "http://127.0.0.1:8200";
            var roleid = "app-vaultrabbit-dev";
            var secretid = "403f475c-7747-1eea-cb4f-98d2e3828ed8";
            var vaultClientSettings = new VaultClientSettings(vaultServerUriWithPort, new AppRoleAuthMethodInfo(
                roleid,
                secretid
            ));
            var vaultClient = new VaultClient(vaultClientSettings);

            // get dynamic secret from vault for rabbitmq
            Secret<UsernamePasswordCredentials> secret = await vaultClient.V1.Secrets.RabbitMQ.GetCredentialsAsync("my-role");
            string username = secret.Data.Username;
            string password = secret.Data.Password;

            // Connect to RabbitMQ with dynamic secret
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 32447, UserName = username, Password = password };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: null,
                                    body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
