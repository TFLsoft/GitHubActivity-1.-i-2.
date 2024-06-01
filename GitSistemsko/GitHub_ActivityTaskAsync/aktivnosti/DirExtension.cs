using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApi_ActivityTaskAsync.aktivnosti
{
    public static class DirExtension
    {
        public static string? ProjectBase()
        {
            var currDir = Directory.GetCurrentDirectory();
            var baseDir = Directory.GetParent(currDir)?.Parent?.Parent?.FullName;
            return baseDir;
        }
    }
}
