using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class SupportController : Controller
{
    private readonly JiraService _jiraService;

    public SupportController(JiraService jiraService)
    {
        _jiraService = jiraService;
    }

    [HttpGet("support/create-ticket")]
    public IActionResult CreateTicketForm()
    {
        return View();
    }

    [HttpPost("support/create-ticket")]
    public async Task<IActionResult> CreateTicket(string summary, string description, string priority)
    {

        var link = Request.Headers["Referer"].ToString();
        var result = await _jiraService.CreateTicket(summary, description, priority, link);
        return RedirectToAction("Index", "MainPage");
    }
}
