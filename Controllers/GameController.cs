using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UTTT.Models;
using Microsoft.EntityFrameworkCore;

namespace UTTT.Controllers;

public class GameInfoViewModel
{
    public Game Game { get; set; } = null!;
    public int TotalMoves { get; set; }
}

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

    public IActionResult Info(int gameId)
    {
        var games = _context.Games
            .Include(g => g.XPlayerNavigation)
            .Include(g => g.OPlayerNavigation)
            .Include(g => g.WinnerNavigation)
            .Where(g => g.Id == gameId)
            .ToList();

        Game game;

        if (games.Count == 0)
        {
            return Redirect("/game/gameerror?msg=Game Not Found!");
        }
        else
        {
            game = games.First();
        }

        var totalMoves = _context.GameMoves
            .Where(gm => gm.GameId == gameId)
            .Count();

        var viewModel = new GameInfoViewModel();
        viewModel.Game = game;
        viewModel.TotalMoves = totalMoves;

        return View(viewModel);
    }

    public IActionResult GameError(string msg)
    {
        ViewData.Add("msg", msg);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
