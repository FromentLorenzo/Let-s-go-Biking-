using System.ServiceModel;
using System.Threading.Tasks;  // Assurez-vous d'importer ce namespace

namespace ServeurSoap
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Task<string> getRoute(string depart, string arrivee);
    }
}
