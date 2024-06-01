using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using aktivnosti;


public class Program
{

    public static readonly HttpClient HttpClient = new();
    public static DateTime PreviousCleanupTime = DateTime.Now;

    public static void Main(string[] args)
    {

     

        var baseDir = DirExtension.ProjectBase();
        if (baseDir != null)
        {
            var path = Path.Combine(baseDir, ".env");
            DotEnv.Inject(path);
        }

        HttpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub_API");
        //var ghToken = "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";/*Environment.GetEnvironmentVariable("GH_TOKEN");*/
        //HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ghToken);
        var ghToken = Environment.GetEnvironmentVariable("GH_TOKEN");
        if (ghToken != null)
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ghToken);


        var listener = new HttpListener();
        listener.Prefixes
                .Add("http://localhost:8080/");
        listener.Start();
        
        Console.WriteLine("Waiting for requests ....");

        while (true)
        {
        
            ThreadPool.QueueUserWorkItem(ServeRequest, listener.GetContext());
        }
    }




    private static void ServeRequest(object? state)
    {
        if (state == null)
            return;

        var context = (HttpListenerContext)state;
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
                var events = FetchEvents(apiUrl);

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
                response.OutputStream.Write(responseByteArray, 0, responseByteArray.Length);

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
    }



    private static List<GitHubEvent> FetchEvents(string apiUrl)
        {
            // Provera da li su podaci već keširani
            var cachedEvents = Cache.GetFromCache(apiUrl);
            if (cachedEvents != null)
            {
            Console.WriteLine("Reading from cache:");
                return cachedEvents;
            }

            // Dohvatanje podataka sa GitHub-a
            var res = HttpClient.GetAsync(apiUrl).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception($"ERROR: {res.StatusCode}");

            var content = res.Content.ReadAsStringAsync().Result;
            var events = JsonConvert.DeserializeObject<List<GitHubEvent>>(content);

            // Dodavanje podataka u keš
            Cache.AddToCache(apiUrl, events);

            return events;
        }

        }