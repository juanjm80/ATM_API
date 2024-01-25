namespace ATP_API.Models.Custom
{
    public class AutorizacionResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Resultado { get; set; }
        public string Msg { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public decimal? Disponible { get; set; }
    }
}
