using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BGS_SitRep
{
    class GetInfo
    {
        private readonly string _baseUri_system = "https://www.edsm.net/api-system-v1/";
        private readonly string _baseUri_sphere = "https://www.edsm.net/api-v1/";

        //Type options are factions, traffic, expansion-info
        public async Task<string> GetSystemInfo(string system)
        {
            string uri = _baseUri_system;
            system = Uri.EscapeDataString(system);
            system = system.Replace("%22", "");
            system = system.Replace("%20", "+");
            string fullUri = uri + "factions?systemName=" + system;
//            System.Console.WriteLine(fullUri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public async Task<string> GetTrafficInfo(string system)
        {
            string uri = _baseUri_system;
            system = system.Replace("%22", "");
            system = system.Replace("%20", "+");
            system = Uri.EscapeUriString(system);
            string fullUri = uri + "traffic?systemName=" + system;
//            System.Console.WriteLine(fullUri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<string> GetExpansionInfo(string system)
        {
            string uri = _baseUri_sphere;
            system = system.Replace("%22", "");
            system = system.Replace("%20", "+");
            system = Uri.EscapeUriString(system);
            string fullUri = uri + "sphere-systems?systemName=" + system + "&radius=20";
//            System.Console.WriteLine(fullUri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

}
