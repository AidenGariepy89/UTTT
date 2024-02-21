using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class Player
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PwdHash { get; set; } = null!;

    public virtual ICollection<Game> GameOPlayerNavigations { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameWinnerNavigations { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameXPlayerNavigations { get; set; } = new List<Game>();
}
