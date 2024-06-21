using global::NYTimesSentimentAnalysis.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace NYTimesSentimentAnalysis.Services;

public class SentimentData
    {
        public string SentimentText { get; set; }
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    public class SentimentAnalysisService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public SentimentAnalysisService()
        {
            _mlContext = new MLContext();
            _model = TrainModel();
        }

        private ITransformer TrainModel()
        {
            var data = new List<SentimentData>
            {
                new SentimentData { SentimentText = "I love this movie" },
                new SentimentData { SentimentText = "I hate this movie" }
            };

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                             .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features"));

            return pipeline.Fit(trainingData);
        }

        public SentimentResult PredictSentiment(string text)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
            var prediction = predictionEngine.Predict(new SentimentData { SentimentText = text });

            return new SentimentResult
            {
                Sentiment = prediction.Sentiment,
                Probability = prediction.Probability,
                Score = prediction.Score
            };
        }
    }


