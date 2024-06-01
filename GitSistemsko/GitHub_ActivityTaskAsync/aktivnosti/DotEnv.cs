using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApi_ActivityTaskAsync.aktivnosti
{
    public class DotEnv
    {
        public static void Inject(string path)
        {
            if (!File.Exists(path))
                return;

            foreach (var line in File.ReadAllLines(path))
            {
                var split = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(split[0], split[1]);
            }
        }
    }
}
