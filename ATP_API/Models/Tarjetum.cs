using System;
using System.Collections.Generic;

namespace ATP_API.Models;

public partial class Tarjetum
{
    public int IdTarjeta { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string NumeroTarjeta { get; set; } = null!;

    public string? Pin { get; set; }

    public string? Codigo { get; set; }

    public bool EsActivo { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public decimal? Disponible { get; set; }

    public decimal? Balance { get; set; }

    public virtual ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; } = new List<HistorialRefreshToken>();

    public virtual ICollection<Retiro> Retiros { get; } = new List<Retiro>();
}
