using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MagazineStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //set api base url
                MagazineStore theStore = new MagazineStore();

                //get token value
                await theStore.GetToken();

                //get all categories
                Category objCategory = await theStore.LoadCategory();

                //loop thru with all categories to get respective list of magazines
                List<MagazineData> magazineData = new List<MagazineData>();
                foreach (string name in objCategory.data)
                {
                    Magazine objMagazine = await theStore.LoadMagazine(name);
                    magazineData.AddRange(objMagazine.data);
                }

                //get all subscribers
                Subscriber objSubscriber = await theStore.LoadSubscriber();

                //to identify the eligible subscribers from the respective collection
                AnswerBody liEligibleSubscriber = new AnswerBody();
                liEligibleSubscriber.subscribers = new List<string>();
                foreach (SubscriberData data in objSubscriber.data)
                {
                    var findmatch = magazineData
                        .Where(m => data.magazineIds
                        .Any(u => u == m.id))
                        .Select(s => s.category)
                        .Distinct().ToList();

                    if (findmatch.Count == objCategory.data.Count)
                    {
                        liEligibleSubscriber.subscribers.Add(data.id);
                    }
                }

                //post the subscribers to answer api call
                AnswerResponse objAnswerResponse = await theStore.PostAnswer(JsonConvert.SerializeObject(liEligibleSubscriber));
                Console.WriteLine(JsonConvert.SerializeObject(objAnswerResponse));

                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }

    public class MagazineStore
    {
        private static string _baseApiUrl;
        private string _token = string.Empty;

        public MagazineStore()
        {
            _baseApiUrl = ConfigurationManager.AppSettings["url.api.magazinestore"];
        }

        private async Task<string> ApiCall(HttpMethod method, string endpoint, string postdata="")
        {
            try
            {
                String strJsonResponse = string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseApiUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = null;
                    if (HttpMethod.Get.Equals(method))
                    {
                        response = await client.GetAsync(endpoint);
                    }
                    else
                    {
                        response = await client.PostAsync(endpoint, new StringContent(postdata, Encoding.UTF8, "application/json"));    
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        strJsonResponse = response.Content.ReadAsStringAsync().Result;
                    }
                    return strJsonResponse;
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            return null;
        }

        public async Task<string> GetToken()
        {
            string strResponse = await ApiCall(HttpMethod.Get, "/api/token");
            if (!string.IsNullOrEmpty(strResponse))
            {
                this._token = JsonConvert.DeserializeObject<Token>(strResponse).token;
            }
            return this._token;
        }

        public async Task<Category> LoadCategory()
        {
            string strResponse = await ApiCall(HttpMethod.Get, string.Format("/api/categories/{0}", this._token));
            if (!string.IsNullOrEmpty(strResponse))
            {
                return JsonConvert.DeserializeObject<Category>(strResponse);
            }
            return null;
        }

        public async Task<Magazine> LoadMagazine(string category)
        {
            string strResponse = await ApiCall(HttpMethod.Get, string.Format("/api/magazines/{0}/{1}", this._token, category));
            if (!string.IsNullOrEmpty(strResponse))
            {
                return JsonConvert.DeserializeObject<Magazine>(strResponse);
            }
            return null;
        }

        public async Task<Subscriber> LoadSubscriber()
        {
            string strResponse = await ApiCall(HttpMethod.Get, string.Format("/api/subscribers/{0}", this._token));
            if (!string.IsNullOrEmpty(strResponse))
            {
                return JsonConvert.DeserializeObject<Subscriber>(strResponse);
            }
            return null;
        }

        public async Task<AnswerResponse> PostAnswer(string postdata)
        {
            string strResponse = await this.ApiCall(HttpMethod.Post, string.Format("/api/answer/{0}", this._token), postdata);
            if (!string.IsNullOrEmpty(strResponse))
            {
                return JsonConvert.DeserializeObject<AnswerResponse>(strResponse);
            }
            return null;
        }

    }

}
