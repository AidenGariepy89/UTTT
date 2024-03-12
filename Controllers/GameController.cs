using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UTTT.Models;

namespace UTTT.Controllers;

public class GameController : Controller
{
    private readonly ILogger<GameController> _logger;
    private readonly UTTTContext _context;

    public GameController(ILogger<GameController> logger, UTTTContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
