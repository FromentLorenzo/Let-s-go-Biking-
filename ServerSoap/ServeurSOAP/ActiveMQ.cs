using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace ServeurSOAP
{
    public class ActiveMQ
    {
        string NomQueue;
        public ActiveMQ(string Nom) {
            this.NomQueue = Nom;
        }
        public async Task<bool> PushOnQueueAsync(string messageTask)
        {
            try
            {
                
                Uri connectUri = new Uri("activemq:tcp://localhost:61616");
                
                ConnectionFactory connectionFactory = new ConnectionFactory(connectUri);
                
                // Créer une connexion unique depuis la fabrique de connexions.
                using (IConnection connection = connectionFactory.CreateConnection())
                {
                    // Créer une session à partir de la connexion.
                    using (ISession session = connection.CreateSession())
                    {
                        // Démarrer la connexion.
                        connection.Start();
                        // Utiliser la session pour cibler une file d'attente.
                        IDestination destination = session.GetQueue(this.NomQueue);
                       
                        // Créer un producteur ciblant la file d'attente sélectionnée.
                        using (IMessageProducer producer = session.CreateProducer(destination))
                        {
                           
                            // Configurer le producteur selon vos besoins.
                            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                           
                            List<string> instructions= new List<string>();
                            
                            instructions =ParseInstructions(messageTask);
                            
                            foreach (string instruction in instructions)
                            {
                                Console.WriteLine(instruction);
                                ITextMessage textmessage= session.CreateTextMessage(instruction);
                                producer.Send(textmessage);
                            }

                            Console.WriteLine($"Message sent on queue {NomQueue}");

                            // N'oubliez pas de fermer votre session et connexion lorsque vous avez terminé.
                            session.Close();
                            connection.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
        }

        public async Task<bool> PurgeQueue()
        {
            try
            {
                Uri connectUri = new Uri("activemq:tcp://localhost:61616");
                ConnectionFactory connectionFactory = new ConnectionFactory(connectUri);

                // Créer une connexion unique depuis la fabrique de connexions.
                using (IConnection connection = connectionFactory.CreateConnection())
                {
                    // Créer une session à partir de la connexion.
                    using (ISession session = connection.CreateSession())
                    {
                        // Utiliser la session pour cibler une file d'attente.
                        IDestination destination = session.GetQueue(this.NomQueue);

                        // Créer un consommateur ciblant la file d'attente sélectionnée.
                        using (IMessageConsumer consumer = session.CreateConsumer(destination))
                        {
                            connection.Start();

                            // Consommer tous les messages existants dans la file d'attente sans les traiter.
                            IMessage message;
                            while ((message = consumer.Receive(TimeSpan.FromMilliseconds(1000))) != null)
                            {
                                Console.WriteLine($"Purged message: {((ITextMessage)message).Text}");
                            }

                            // N'oubliez pas de fermer votre session et connexion lorsque vous avez terminé.
                            session.Close();
                            connection.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
        }

        public List<string> ParseInstructions(string instructions)
        {
            // Diviser la chaîne en lignes
            string[] lines = instructions.Split('\n');

            // Convertir le tableau de lignes en liste
            List<string> instructionList = new List<string>(lines);

            return instructionList;
        }
    }
}
