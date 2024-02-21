using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class Game
{
    public int Id { get; set; }

    public int XPlayer { get; set; }

    public int OPlayer { get; set; }

    public int? Winner { get; set; }

    public virtual ICollection<GameMove> GameMoves { get; set; } = new List<GameMove>();

    public virtual Player OPlayerNavigation { get; set; } = null!;

    public virtual Player? WinnerNavigation { get; set; }

    public virtual Player XPlayerNavigation { get; set; } = null!;
}
