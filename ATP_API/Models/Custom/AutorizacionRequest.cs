namespace ATP_API.Models.Custom
{
    public class AutorizacionRequest
    {
        public string NumeroTarjeta { get; set; }
        public string Pin { get; set; }
        public decimal? Retiro { get; set; }
    }
}
