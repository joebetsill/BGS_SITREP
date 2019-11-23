using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BGS_SitRep
{
    class GetInfo
    {
        private string _base_uri = "https://www.edsm.net/api-system-v1/";

        //Type options are factions, traffic
        public async Task<string> GetSystemInfo(string system)
        {
            string uri = _base_uri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri+"factions?systemName="+system);
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
            string uri = _base_uri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + "traffic?systemName=" + system);
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
            string uri = _base_uri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + "sphere-systems?systemName=" + system + "&radius=20");
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
