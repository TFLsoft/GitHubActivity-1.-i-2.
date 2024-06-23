using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ConsoleApp1;
class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "rOOVc3tQDBWxAtUzlubwY7ZjxAh2HyVf";
        string apiUrl = $"https://api.nytimes.com/svc/mostpopular/v2/viewed/1.json?api-key={apiKey}";

        try
        {
            var articles = await GetArticlesAsync(apiUrl);

            IObservable<Article> articleObservable = articles.ToObservable();

            articleObservable
                .Select(article =>
                {
                    var sentiment = AnalyzeSentiment(article.Title + " " + article.Abstract);
                    return new { Article = article, Sentiment = sentiment };
                })
                .Subscribe(result =>
                {
                    Console.WriteLine($"Title: {result.Article.Title}");
                    Console.WriteLine($"Abstract: {result.Article.Abstract}");
                    Console.WriteLine($"Sentiment: {result.Sentiment}");
                    Console.WriteLine();
                });

            Console.ReadLine();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }
    }

    static async Task<List<Article>> GetArticlesAsync(string url)
    {
        using HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse>(responseString);
        return data.Results;
    }

    static string AnalyzeSentiment(string text)
    {
        // Jednostavan način za procenu sentimenta
        if (text.ToLower().Contains("good") || text.ToLower().Contains("amazing") || text.ToLower().Contains("poor") || text.ToLower().Contains("excellent"))
        {
            return "Positive";
        }
        else if (text.ToLower().Contains("bad") || text.ToLower().Contains("dark") || text.ToLower().Contains("sad") || text.ToLower().Contains("poor") || text.ToLower().Contains("terrible"))
        {
            return "Negative";
        }
        else
        {
            return "Neutral";
        }
    }
}
