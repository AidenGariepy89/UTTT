using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTTT.Models;

namespace UTTT.Controllers;

public class SimplePlayer
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
}

public class IndexViewModel
{
    public List<SimplePlayer> PlayersInLobby { get; set; } = null!;
    public List<SimplePlayer> PlayersInGame { get; set; } = null!;
    public List<SimplePlayer> Players { get; set; } = null!;
}

public class InfoViewModel
{
    public Player Player { get; set; } = null!;
    public List<Game> Games { get; set; } = null!;
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
}

public class PlayerController : Controller
{
    private readonly ILogger<GameController> _logger;
    private readonly UTTTContext _context;


    public PlayerController(ILogger<GameController> logger, UTTTContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var playersInLobby = _context.Database.SqlQuery<SimplePlayer>(
            $@"select p.id, p.username
            from player p, conn c, lobby l
            where l.conn_id = c.conn_id
            and c.player_id = p.id"
        ).ToList();

        var playersInGame = _context.Database.SqlQuery<SimplePlayer>(
            $@"select p.id, p.username
            from player p, conn c
            where c.player_id = p.id
            except
            select p.id, p.username
            from player p, conn c, lobby l
            where l.conn_id = c.conn_id
            and c.player_id = p.id"
        ).ToList();

        var players = _context.Database.SqlQuery<SimplePlayer>(
            $@"select *
            from player"
        ).ToList();

        var viewModel = new IndexViewModel();
        viewModel.PlayersInLobby = playersInLobby;
        viewModel.PlayersInGame = playersInGame;
        viewModel.Players = players;

        return View(viewModel);
    }

    public IActionResult Info(int id)
    {
        var player = _context.Players.Find(id);
        if (player is null)
        {
            return Redirect("/game/gameerror?msg=Player not found!");
        }

        var gamesPlayed = _context.Games
            .Include(g => g.XPlayerNavigation)
            .Include(g => g.OPlayerNavigation)
            .Where(g => g.XPlayer == id || g.OPlayer == id)
            .ToList();
        if (gamesPlayed is null)
        {
            gamesPlayed = new List<Game>();
        }

        int gamesWon = 0;
        int gamesLost = 0;
        foreach (var game in gamesPlayed)
        {
            if (game.Winner is not null)
            {
                if (game.Winner == player.Id)
                {
                    gamesWon++;
                }
                else
                {
                    gamesLost++;
                }
            }
        }

        var viewModel = new InfoViewModel();
        viewModel.Player = player;
        viewModel.Games = gamesPlayed;
        viewModel.GamesWon = gamesWon;
        viewModel.GamesLost = gamesLost;

        return View(viewModel);
    }
}
