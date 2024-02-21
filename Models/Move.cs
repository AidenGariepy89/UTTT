using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class Move
{
    public int Id { get; set; }

    public int Board { get; set; }

    public int Cell { get; set; }

    public string Piece { get; set; } = null!;

    public virtual ICollection<GameMove> GameMoves { get; set; } = new List<GameMove>();
}
