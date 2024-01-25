using System;
using System.Collections.Generic;

namespace ATP_API.Models;

public partial class HistorialRefreshToken
{
    public int IdHistorialToken { get; set; }

    public int? IdTarjeta { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual Tarjetum? IdTarjetaNavigation { get; set; }
}
