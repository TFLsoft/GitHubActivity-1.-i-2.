using Microsoft.AspNetCore.Mvc;
using NYTimesSentimentAnalysis.Model;
using NYTimesSentimentAnalysis.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NYTimesSentimentAnalysis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;
        private readonly SentimentAnalysisService _sentimentService;

        public ArticlesController()
        {
            _articleService = new ArticleService();
            _sentimentService = new SentimentAnalysisService();
        }

        [HttpGet]
        public async Task<IEnumerable<Article>> Get()
        {
            var articles = await _articleService.GetArticlesAsync();
            var observableArticles = articles.ToObservable();
            var analyzedArticles = await observableArticles
                .Select(article =>
                {
                    var sentimentText = article.Title + " " + article.Abstract;
                    article.SentimentResult = _sentimentService.PredictSentiment(sentimentText);
                    return article;
                })
                .ToList();

            return analyzedArticles;
        }
    }
}
