﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTTT.Models;

public partial class Player
{
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Conn> Conns { get; set; } = new List<Conn>();

    public virtual ICollection<Game> GameOPlayerNavigations { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameWinnerNavigations { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameXPlayerNavigations { get; set; } = new List<Game>();
}
