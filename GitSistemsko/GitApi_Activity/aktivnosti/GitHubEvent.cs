namespace aktivnosti;

public class GitHubEvent
{
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public  Author Actor { get; set; }
}


