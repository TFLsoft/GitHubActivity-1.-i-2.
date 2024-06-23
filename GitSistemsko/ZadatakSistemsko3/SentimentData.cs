using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadatakSistemsko3
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? Sentiment { get; set; }

        [LoadColumn(1)]
        public string? SentimentText { get; set; }
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public string? Prediction { get; set; }
    }

}
