using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class Conn
{
    public string ConnId { get; set; } = null!;

    public int PlayerId { get; set; }

    public virtual Lobby? Lobby { get; set; }

    public virtual Player Player { get; set; } = null!;
}
