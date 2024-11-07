using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JiraService
{
    private readonly string _jiraUrl = "https://customformsapp.atlassian.net";
    private readonly string _username = "maksimgornostaev197@gmail.com";
    private readonly string _apiToken = "ATATT3xFfGF09jYxjzNzY5ZENDtzmM-g9YwtG_I-qWhaeYL-j2nQCE6uQGTJcaTn9hwE8FAMARyNbBuXUCEm7UHL0ua__z97uLp7YftS7ZLuXT8uKZK7AvYsFowgneQULCcZHHtRVG7JuSgWgM8Qo_m_tT-uRWmfjN3MjLEPVMLM2SZPafKmJ4U=4C6DD3EE";
    private readonly HttpClient _client;

    public JiraService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_apiToken}")));
    }

    public async Task<string> CreateTicket(string summary, string description, string priority, string link)
    {
        var ticketData = new
        {
            fields = new
            {
                project = new { key = "ST" },
                summary = summary,
                description = description + "\n\nLink: " + link,
                issuetype = new { name = "Task" },
                priority = new { name = priority }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(ticketData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"{_jiraUrl}/rest/api/2/issue", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            throw new Exception("Ошибка при создании тикета: " + response.ReasonPhrase);
        }
    }

    public async Task<List<string>> GetTicketsForUser(string userId)
    {
        var tickets = new List<string>();

        var response = await _client.GetAsync($"{_jiraUrl}/rest/api/2/search?jql=reporter={userId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(result);

            foreach (var issue in jsonResponse["issues"])
            {
                tickets.Add(issue["self"].ToString());
            }
        }

        return tickets;
    }
}