using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadatakSistemsko3
{
    public class ApiResponse
    {
        [JsonProperty("results")]
        public List<Article> Results { get; set; }
    }


}
