using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProxySoap
{
    public class JCDecauxItems
    {
        public static readonly HttpClient client = new HttpClient();
        public string url = "https://api.jcdecaux.com/vls/v3/";
        public string apiKey = "e65de5172e58282b856fd72204eb35c710d1e336";
        public static string allStationsOfAContract;
        public string contractSelected;

        public JCDecauxItems(string contractSelected)
        {
            this.contractSelected = contractSelected;
            allStationsOfAContract = callJCD(url + "stations?contract=" + contractSelected + "&apiKey=" + apiKey).Result;

        }

        public static async Task<string> callJCD(string request)
        {
            try
            {
                string responseBody = await client.GetStringAsync(request);
                return responseBody;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public string getStations()
        {
            return allStationsOfAContract;
        }
    }
}
