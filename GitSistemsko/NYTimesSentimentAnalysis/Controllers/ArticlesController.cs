using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NYTimesSentimentAnalysis.Model;
using NYTimesSentimentAnalysis.Services;
using Microsoft.Extensions.Logging;


    [ApiController]
    [Route("[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;
        private readonly SentimentAnalysisService _sentimentService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ILogger<ArticlesController> logger)
        {
            _articleService = new ArticleService();
            _sentimentService = new SentimentAnalysisService();
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Article>> Get()
        {
            _logger.LogInformation("Received request for articles.");

            // Dobijamo listu članaka
            var articles = await _articleService.GetArticlesAsync();

            // Konvertujemo listu u observable
            var observableArticles = articles.ToObservable();

            // Radimo sa observable i vršimo sentiment analizu
            var analyzedArticles = await observableArticles
                .Select(article =>
                {
                    var sentimentText = article.Title + " " + article.Abstract;
                    article.SentimentResult = _sentimentService.PredictSentiment(sentimentText);
                    return article;
                })
                .ToList();

            _logger.LogInformation("Returning analyzed articles.");
            return analyzedArticles;
        }
    }
