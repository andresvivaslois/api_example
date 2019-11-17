using GNB.Domain.Helpers.Interfaces;
using GNB.Utilities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GNB.Domain.Helpers
{
    public class RequestHelper : IRequestHelper
    {
        public async Task<List<T>> GetListFromUrl<T>(string url)
        {
            var lResult = new List<T>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var test = response.Content.ReadAsStringAsync();

                        lResult = JsonConvert.DeserializeObject<List<T>>(test.Result);
                    }
                    else
                    {
                        LoggerHelper.LogError($"Error in request. Error code: {response.StatusCode}");
                    }

                    return lResult;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return new List<T>();
            }


        }
    }
}
