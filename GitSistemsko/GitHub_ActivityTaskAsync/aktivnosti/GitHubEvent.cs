using GitHubApi_ActivityTaskAsync.aktivnosti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApi_ActivityTaskAsync.aktivnosti
{
    public class GitHubEvent
    {
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public Author Actor { get; set; }
    }
}
