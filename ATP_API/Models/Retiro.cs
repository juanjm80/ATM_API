using System;
using System.Collections.Generic;

namespace ATP_API.Models;

public partial class Retiro
{
    public int IdRetiro { get; set; }

    public int IdTarjeta { get; set; }

    public decimal? Retiro1 { get; set; }

    public string? CodigoOperacion { get; set; }

    public DateTime? FechaExtraccion { get; set; }

    public virtual Tarjetum IdTarjetaNavigation { get; set; } = null!;
}
