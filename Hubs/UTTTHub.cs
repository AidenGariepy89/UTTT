using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UTTT.Models;

public class UTTTHub : Hub
{
    private UTTTContext _context;

    public UTTTHub(UTTTContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Hello there: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Goodbye: {Context.ConnectionId} | {exception}");

        // TODO: make these sorts of things DELETE statements
        var lobby = await _context.Lobbies.FindAsync(Context.ConnectionId);

        if (lobby is not null)
        {
            _context.Lobbies.Remove(lobby);
            await _context.SaveChangesAsync();
        }

        await _context.Conns
            .FromSql($"select * from conn where conn_id = {Context.ConnectionId}")
            .ForEachAsync(conn => _context.Conns.Remove(conn));

        await _context.SaveChangesAsync();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterUsername(string username)
    {
        if (username == "")
        {
            Console.WriteLine("Empty username!");
            await Clients.Caller.SendAsync("clientError", "Empty username!");
            return;
        }

        // Check if player exists
        var playerQuery = await _context.Players
            .FromSql($"select * from player where username = {username}")
            .ToListAsync();

        Player player;

        if (playerQuery.Count() == 0)
        {
            player = new Player();
            player.Id = _context.Players.Count() + 1;
            player.Username = username;

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }
        else
        {
            player = playerQuery.First();
        }

        // Delete existing connections for this player
        // THIS WILL MOST LIKELY CAUSE FUTURE PROBLEMS FOR MYSELF
        var conns = await _context.Conns
            .FromSql($"select * from conn where player_id = {player.Id}")
            .ToListAsync();
        foreach (var conn in conns)
        {
            var lobby = await _context.Lobbies.FindAsync(conn.ConnId);
            if (lobby is not null)
            {
                _context.Lobbies.Remove(lobby);
            }
        }
        conns.ForEach(conn => _context.Conns.Remove(conn));

        Conn c = new Conn();
        c.ConnId = Context.ConnectionId;
        c.Player = player;

        _context.Conns.Add(c);

        JoinLobby(c);

        await _context.SaveChangesAsync();

        await MatchMaking(c);
    }

    public async Task SubmitMove(string json)
    {
        Console.WriteLine(json);

        await Clients.All.SendAsync("announceMove", json);
    }

    public async Task LeaveLobby()
    {
        Lobby? lobby = await _context.Lobbies.FindAsync(Context.ConnectionId);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("clientError", "Not in lobby!");
            return;
        }
        _context.Lobbies.Remove(lobby);
        await _context.SaveChangesAsync();

        await Clients.Caller.SendAsync("serverMsg", "Left lobby");
    }

    private void JoinLobby(Conn conn)
    {
        if (_context.Lobbies.Find(conn.ConnId) is not null)
        {
            return;
        }

        Lobby lobby = new Lobby();
        lobby.Conn = conn;
        _context.Lobbies.Add(lobby);
    }

    private async Task MatchMaking(Conn conn)
    {
        var waitingForMatchQuery = _context.Lobbies
            .FromSql($"select * from lobby where conn_id != {Context.ConnectionId}");

        if (waitingForMatchQuery.Count() == 0)
        {
            return;
        }

        Lobby waitingForMatch = await waitingForMatchQuery.FirstAsync();
        Console.WriteLine("------------------------------- " + waitingForMatch.ConnId);
        Conn? otherPlayer = _context.Conns.Find(waitingForMatch.ConnId);
        if (otherPlayer is null)
        {
            return;
        }

        await MatchGame(conn, otherPlayer);
    }

    private async Task MatchGame(Conn conn, Conn otherPlayer)
    {
        var game = new Game();
        game.Id = _context.Games.Count() + 1;
        game.XPlayer = otherPlayer.PlayerId;
        game.OPlayer = conn.PlayerId;

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        await Clients.Client(otherPlayer.ConnId).SendAsync("joinGame", "X");
        await Clients.Caller.SendAsync("joinGame", "O");
    }
}
