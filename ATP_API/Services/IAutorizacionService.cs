using ATP_API.Models.Custom;

namespace ATP_API.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion);
        Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest refreshTokenRequest, int idTarjeta);
        Task<TarjetaResponse> DevolverTarjeta(TarjetaRequest autorizacion);
        Task<TarjetaResponse> BloquearTarjeta(TarjetaRequest bloquear);
        Task<TarjetaResponse> RetirarDinero(AutorizacionRequest retirar);
    }
}
