using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace NYTimesSentimentAnalysis.Services
{
    public class SentimentAnalysisService
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public SentimentAnalysisService()
        {
            _mlContext = new MLContext(seed: 0);
            TrainModel();
        }

        public SentimentPrediction PredictSentiment(string text)
        {
            var input = new SentimentData { SentimentText = text };
            var prediction = _predictionEngine.Predict(input);
            return prediction;
        }

        private void TrainModel()
        {
            var data = _mlContext.Data.LoadFromTextFile<SentimentData>(@"path\to\your\training\data.csv", hasHeader: true, separatorChar: ',');

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimentData.Label), featureColumnName: "Features"));

            var trainedModel = pipeline.Fit(data);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(trainedModel);
        }
    }

    public class SentimentData
    {
        [LoadColumn(0)] // Indeks kolone u dataset-u
        public string SentimentText { get; set; }

        [LoadColumn(1)] // Indeks kolone u dataset-u koja predstavlja oznaku (label)
        public bool Label { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
    }

    
}
