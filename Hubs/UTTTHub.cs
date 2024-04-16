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

        var oldConn = await _context.Conns.FindAsync(Context.ConnectionId);
        if (oldConn is not null)
        {
            _context.Conns.Remove(oldConn);
        }

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

        Player player;
        var playerQuery = await _context.Players.Where(p => p.Username == username).ToListAsync();
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
        int gameId;
        Move move;

        if (!Utils.UnmarshalMovePacket(json, out gameId, out move))
        {
            throw new Exception("SubmitMove packet in invalid format!");
        }

        Game? game = await _context.Games.FindAsync(gameId);
        if (game is null)
        {
            await Clients.Caller.SendAsync("clientError", "Unknown game id");
            return;
        }

        await SaveMove(game, move);

        Conn?[] conns = [
            game.GetXPlayer(_context).GetConn(_context),
            game.GetOPlayer(_context).GetConn(_context),
        ];
        bool playerDisconnected = false;
        var existingConns = new List<Conn>();

        foreach (var conn in conns)
        {
            if (conn is null)
            {
                playerDisconnected = true;
            }
            else
            {
                existingConns.Add(conn);
            }
        }

        // Handle disconnection
        if (playerDisconnected)
        {
            foreach (var conn in existingConns)
            {
                await Clients.Client(conn.ConnId).SendAsync("clientError", "Player disconnected");
            }
            return;
        }

        foreach (var conn in existingConns)
        {
            await Clients.Client(conn.ConnId).SendAsync("announceMove", json);
        }
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

        string xPlayerPacket = Utils.CreateJoinGamePacket(game, "X");
        string oPlayerPacket = Utils.CreateJoinGamePacket(game, "O");

        await Clients.Client(otherPlayer.ConnId).SendAsync("joinGame", xPlayerPacket);
        await Clients.Caller.SendAsync("joinGame", oPlayerPacket);
    }

    private async Task SaveMove(Game game, Move move)
    {
        var moveQuery = _context.Moves
            .FromSql($"select * from moves where board = {move.Board} and cell = {move.Cell} and piece = {move.Piece}");

        Move moveToUse;

        if (await moveQuery.CountAsync() == 0)
        {
            moveToUse = move;
            moveToUse.Id = await _context.Moves.CountAsync() + 1;
            _context.Moves.Add(moveToUse);
        }
        else
        {
            moveToUse = await moveQuery.FirstAsync();
        }

        int movesInGame = await _context.GameMoves.FromSql($"select * from game_moves where game_id = {game.Id}").CountAsync();

        var gm = new GameMove();
        gm.Id = await _context.GameMoves.CountAsync() + 1;
        gm.GameId = game.Id;
        gm.MoveId = moveToUse.Id;
        gm.Turn = movesInGame + 1;

        _context.GameMoves.Add(gm);

        await _context.SaveChangesAsync();
    }
}
