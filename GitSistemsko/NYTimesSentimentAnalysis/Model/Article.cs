using Microsoft.ML.Data;

namespace NYTimesSentimentAnalysis.Model;

    public class Article
    {
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Url { get; set; }
        public SentimentResult SentimentResult { get; set; }
    }

    public class SentimentResult
    {
        public bool Sentiment { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }



