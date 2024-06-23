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
        [LoadColumn(0), ColumnName("Label")]
        public Boolean Label { get; set; }

        [LoadColumn(1)]
        public string SentimentText { get; set; }
    }

}
