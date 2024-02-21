using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class GameMove
{
    public int GameId { get; set; }

    public int MoveId { get; set; }

    public int Turn { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Move Move { get; set; } = null!;
}
