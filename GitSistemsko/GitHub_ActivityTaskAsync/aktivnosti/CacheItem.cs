using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApi_ActivityTaskAsync.aktivnosti
{
    public class CacheItem
    {
        public List<GitHubEvent> Data { get; set; }
        public DateTime Timestamp { get; set; }

        public CacheItem(List<GitHubEvent> lista)
        {
            Timestamp = DateTime.Now;
            Data = lista;
        }
    }
}
