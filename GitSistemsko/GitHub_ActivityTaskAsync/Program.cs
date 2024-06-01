using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using GitHubApi_ActivityTaskAsync.aktivnosti;
using System.Threading.Tasks;

public class Program
{
    public static readonly HttpClient HttpClient = new();
    public static DateTime PreviousCleanupTime = DateTime.Now;

    public static async Task Main(string[] args)
    {
        var baseDir = DirExtension.ProjectBase();
        if (baseDir != null)
        {
            var path = Path.Combine(baseDir, ".env");
            DotEnv.Inject(path);
        }

        HttpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub_API");
        var ghToken = Environment.GetEnvironmentVariable("GH_TOKEN");
        if (ghToken != null)
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ghToken);

        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Waiting for requests ....");

        while (true)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => ServeRequest(context));
        }
    }

    private static async Task ServeRequest(HttpListenerContext context)
    {
        var response = context.Response;
        try
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var query = context.Request.Url?.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var vars = query.Substring(1)?.Split("&");
                if (vars == null || vars.Length != 2)
                {
                    throw new Exception("Invalid query parameters. You must provide 'owner' and 'repo' parameters.");
                }

                var owner = vars[0].Split("=")[1];
                var repo = vars[1].Split("=")[1];
                var key = $"{owner}/{repo}";

                var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/events";
                var events = await FetchEvents(apiUrl);

                // Generisanje odgovora
                var responseObject = new
                {
                    Owner = owner,
                    Repo = repo,
                    Events = events
                };

                var responseJson = JsonConvert.SerializeObject(responseObject);
                var responseByteArray = Encoding.UTF8.GetBytes(responseJson);
                response.ContentLength64 = responseByteArray.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(responseByteArray, 0, responseByteArray.Length);

                stopwatch.Stop();

                Console.WriteLine($"Request for owner '{owner}' and repo '{repo}' processed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error processing request: {e.Message}");
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.StatusDescription = e.Message;
        }
        finally
        {
            response.OutputStream.Close();
        }
    }

    private static async Task<List<GitHubEvent>> FetchEvents(string apiUrl)
    {
        // Provera da li su podaci već keširani
        var cachedEvents = Cache.GetFromCache(apiUrl);
        if (cachedEvents != null)
        {
            Console.WriteLine("Reading from cache:");
            return cachedEvents;
        }

        // Dohvatanje podataka sa GitHub-a
        var res = await HttpClient.GetAsync(apiUrl);
        if (!res.IsSuccessStatusCode)
            throw new Exception($"ERROR: {res.StatusCode}");

        var content = await res.Content.ReadAsStringAsync();
        var events = JsonConvert.DeserializeObject<List<GitHubEvent>>(content);

        // Dodavanje podataka u keš
        Cache.AddToCache(apiUrl, events);

        return events;
    }
}