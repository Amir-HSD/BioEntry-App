using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Media.Animation;

namespace BioEntry_App
{
    public static class ApiService
    {
        private static HttpClient client;
        static ApiService()
        {
            client = new HttpClient();
        }

        public static async Task<bool> CheckStatusAsync(string Url)
        {
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync(Url);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public static async Task<string> SendRequestAsync(string Url)
        {
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync(Url);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

    }
}
