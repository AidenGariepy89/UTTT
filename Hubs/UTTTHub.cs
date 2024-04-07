using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UTTT.Models;

public class UTTTHub : Hub
{
    private UTTTContext _context;

    public UTTTHub(UTTTContext context) {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Hello there: {Context.ConnectionId}");
        // await Groups.AddToGroupAsync(Context.ConnectionId, "lobby"); 
        // Lobby.Add(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Goodbye: {Context.ConnectionId} | {exception}");

        var connQuery = _context.Conns
            .FromSql($"select * from conn where conn_id = {Context.ConnectionId}")
            .ToList();

        if (connQuery.Count() != 0) {
            foreach (var conn in connQuery) {
                _context.Conns.Remove(conn);
            }
            await _context.SaveChangesAsync();
        }

        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task RegisterUsername(string username)
    {
        // Check if player exists
        var playerQuery = _context.Players
            .FromSql($"select * from player where username = {username}")
            .ToList();

        Player player;

        if (playerQuery.Count() == 0) {
            player = new Player();
            player.Id = _context.Players.Count() + 1;
            player.Username = username;

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        } else {
            player = playerQuery.First();
        }

        // Delete existing connections for this player
        // THIS WILL MOST LIKELY CAUSE FUTURE PROBLEMS FOR MYSELF
        await _context.Conns
            .FromSql($"select * from conn where player_id = {player.Id}")
            .ForEachAsync((conn) => _context.Conns.Remove(conn));

        Conn c = new Conn();
        c.ConnId = Context.ConnectionId;
        c.Player = player;

        _context.Conns.Add(c);
        await _context.SaveChangesAsync();
    }

    public async Task SubmitMove(string json)
    {
        Console.WriteLine(json);

        await Clients.All.SendAsync("announceMove", json);
    }
}
