using ConsoleApp1.ServiceReference1;
using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new Service1Client();

            try
            {
                string resultat = client.getRoute("Avenue Tony Garnier, Lyon", "Avenue des Frères Lumière, Lyon");
                Console.WriteLine("Résultat de la méthode : " + resultat);
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
