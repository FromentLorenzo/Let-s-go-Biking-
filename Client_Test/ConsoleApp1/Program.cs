using ConsoleApp1.ServiceReference1;
using ConsoleApp1.ServiceReference2;
using System;
using System.Text.Json; // Ajoutez cette directive pour utiliser le support de désérialisation JSON

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new Service1Client();
            var client2 = new ProxyClient();

            try
            {
                //string resultat = client.getRoute("Rue Gasparin, Lyon", "Rue Émile Zola, Lyon");
                string resultat = client.getRoute("Rue du Boeuf, Lyon", "Avenue Berthelot, Lyon");
                Console.WriteLine("Résultat de la méthode : " + resultat);
                //Console.WriteLine("Résultat de la méthode : " + resultat2);
                while (!Console.KeyAvailable)
                {
                    // Vous pouvez mettre ici le code que vous souhaitez exécuter continuellement
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
            finally
            {
                // Assurez-vous de fermer le client après utilisation.
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    {
                        client.Abort();
                    }
                    else
                    {
                        client.Close();
                    }
                }
            }
        }
    }
}
