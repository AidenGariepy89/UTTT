using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UTTT.Models;

namespace UTTT.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UTTTContext _context;

    public HomeController(ILogger<HomeController> logger, UTTTContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        List<Player> players = _context.Players.ToList();

        return View(players);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
