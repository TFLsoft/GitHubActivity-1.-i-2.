using Microsoft.ML;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reactive.Linq;
using ZadatakSistemsko3;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "rOOVc3tQDBWxAtUzlubwY7ZjxAh2HyVf";
        string apiUrl = $"https://api.nytimes.com/svc/mostpopular/v2/viewed/1.json?api-key={apiKey}";

        try
        {
            var articles = await GetArticlesAsync(apiUrl);
            var articleObservables = articles.ToObservable();

            // Kreiranje MLContext
            var mlContext = new MLContext(seed: 0);

            // Obuka modela
            var model = TrainModel(mlContext);

            var sentimentResults = articleObservables
                .Select(article =>
                {
                    var sentiment = PredictSentiment(mlContext, model, article.Title + " " + article.Abstract);
                    return new { Article = article, Sentiment = sentiment };
                });

            sentimentResults
                .Subscribe(result =>
                {
                    Console.WriteLine($"Title: {result.Article.Title}");
                    Console.WriteLine($"Abstract: {result.Article.Abstract}");
                    Console.WriteLine($"Sentiment: {result.Sentiment.Prediction}");
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

    static ITransformer TrainModel(MLContext mlContext)
    {
        var dataPath = @"C:\Zadaci\ZadatakSistemsko3\bin\Debug\net8.0\sentiment_labelled_sentences\sentiment.txt";

        if (!File.Exists(dataPath))
        {
            throw new FileNotFoundException($"File not found at path: {dataPath}");
        }

        var dataView = mlContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: false);

        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(SentimentData.Label)))
            .Append(mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(mlContext.Transforms.Concatenate("Features", "Features"))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

        var model = pipeline.Fit(dataView);

        return model;
    }
    

    static SentimentPrediction PredictSentiment(MLContext mlContext, ITransformer model, string text)
    {
        // Predviđanje sentimenta korišćenjem obučenog modela
        var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

        var input = new SentimentData { SentimentText = text };
        var prediction = predictionEngine.Predict(input);

        return prediction;
    }
}