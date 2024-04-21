using System.Text.Json;
using UTTT.Models;

public static class Utils
{
    private class MovePacket
    {
        public int gameId { get; set; }
        public int board { get; set; }
        public int cell { get; set; }
        public string playerPiece { get; set; } = null!;
    }

    private class GameOverPacket
    {
        public int gameId { get; set; }
        public string playerPiece { get; set; } = null!;
    }

    public static string CreateJoinGamePacket(Game game, string playerPiece)
    {
        return "{\"gameId\":" + game.Id + ",\"playerPiece\":\"" + playerPiece + "\"}";
    }

    public static string MarshalMovePacket(int gameId, Move move)
    {
        var packet = new MovePacket();
        packet.gameId = gameId;
        packet.board = move.Board;
        packet.cell = move.Cell;
        packet.playerPiece = move.Piece;

        return JsonSerializer.Serialize(packet);
    }

    public static bool UnmarshalMovePacket(string json, out int gameId, out Move move)
    {
        gameId = -1;
        move = new Move();

        var packet = JsonSerializer.Deserialize<MovePacket>(json);
        if (packet is null)
        {
            return false;
        }

        gameId = packet.gameId;
        move.Board = packet.board;
        move.Cell = packet.cell;
        move.Piece = packet.playerPiece;

        return true;
    }

    public static bool UnmarshalGameOverPacket(string json, out int gameId, out string winner)
    {
        gameId = -1;
        winner = "";

        var packet = JsonSerializer.Deserialize<GameOverPacket>(json);
        if (packet is null)
        {
            return false;
        }

        gameId = packet.gameId;
        winner = packet.playerPiece;

        return true;
    }
}
