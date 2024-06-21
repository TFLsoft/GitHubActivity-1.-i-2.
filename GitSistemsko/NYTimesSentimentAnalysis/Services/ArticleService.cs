using Newtonsoft.Json.Linq;
using NYTimesSentimentAnalysis.Model;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NYTimesSentimentAnalysis.Services;



    public class ArticleService
    {
        private readonly string ApiKey = "YOUR_NYTIMES_API_KEY";
        private readonly string ApiUrl = "https://api.nytimes.com/svc/mostpopular/v2/viewed/1.json?api-key=";

        public async Task<List<Article>> GetArticlesAsync()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(ApiUrl + ApiKey);
            var json = JObject.Parse(response);
            var results = (JArray)json["results"];

            var articles = new List<Article>();
            foreach (var result in results)
            {
                articles.Add(new Article
                {
                    Title = result["title"].ToString(),
                    Abstract = result["abstract"].ToString(),
                    Url = result["url"].ToString()
                });
            }
            return articles;
        }
    }


