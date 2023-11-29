using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxySoap
{
    [ServiceContract]
    public interface IProxy
    {

        [OperationContract]
        string getStation(int number, string chosenContract);

        [OperationContract]
        string getAllStationsOfAContract(string chosenContract);

    }
}
