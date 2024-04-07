using System;
using System.Collections.Generic;

namespace UTTT.Models;

public partial class Lobby
{
    public string ConnId { get; set; } = null!;

    public virtual Conn Conn { get; set; } = null!;
}
