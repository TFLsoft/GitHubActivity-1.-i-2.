using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadatakSistemsko3
{
    public class Article
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("abstract")]
        public string Abstract { get; set; }
    }
}
