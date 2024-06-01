﻿namespace aktivnosti;

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