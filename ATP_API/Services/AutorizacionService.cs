using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ATP_API.Models;
using ATP_API.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace ATP_API.Services
{
    public class AutorizacionService: IAutorizacionService
    {
        private readonly DbatpContext _context;
        private readonly IConfiguration _configuration;

        public AutorizacionService(DbatpContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerarToken(string idTarjeta)
        {
            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idTarjeta));

            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = credencialesToken
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return tokenCreado;
        }

        private string GenerarRefreshToken()
        {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }
            return refreshToken;
        }

        private async Task<AutorizacionResponse> GuardarHistorialRefreshToken(
            int idTarjeta,
            string token,
            string refreshToken
            )
        {
            var historialRefreshToken = new HistorialRefreshToken
            {
                IdTarjeta = idTarjeta,
                Token = token,
                RefreshToken = refreshToken,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(20)
            };

            var tarjeta = _context.Tarjeta.FirstOrDefault(x => x.IdTarjeta == idTarjeta);

            await _context.HistorialRefreshTokens.AddAsync(historialRefreshToken);
            await _context.SaveChangesAsync();

            return new AutorizacionResponse { Token = token, RefreshToken = refreshToken, Resultado = true, Msg = "Ok", FechaExpiracion = tarjeta.FechaExpiracion, Disponible = tarjeta.Disponible };
        }

        public async Task<TarjetaResponse> BloquearTarjeta(TarjetaRequest bloquear)
        {
            var tarjeta = _context.Tarjeta.FirstOrDefault(x => x.NumeroTarjeta == bloquear.NumeroTarjeta);
            tarjeta.EsActivo = false;

            await _context.SaveChangesAsync();

            return new TarjetaResponse { Resultado = true, Msg = "Tarjeta Bloqueada" };
        }

        public async Task<TarjetaResponse> RetirarDinero(AutorizacionRequest retirar)
        {
            var tarjeta = _context.Tarjeta.FirstOrDefault(x => x.NumeroTarjeta == retirar.NumeroTarjeta && x.Pin == retirar.Pin && x.EsActivo);
            if (tarjeta.Disponible >= retirar.Retiro && tarjeta.Balance >= retirar.Retiro)
            {
                tarjeta.Disponible = tarjeta.Disponible - retirar.Retiro;

                Random rnd = new Random();
                int op = rnd.Next(1, 13);

                var retiro = new Retiro
                {
                    IdTarjeta = tarjeta.IdTarjeta,
                    Retiro1 = retirar.Retiro,
                    CodigoOperacion = "Operacion " + op.ToString(),
                    FechaExtraccion = DateTime.UtcNow
                };
                await _context.Retiros.AddAsync(retiro);
                await _context.SaveChangesAsync();

                return new TarjetaResponse { Resultado = true, Msg = "Operacin Completada con Exito." };
            }

            return new TarjetaResponse { Resultado = false, Msg = "La Operacin no se pudo realizar ." };
        }

        public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion)
        {
            var usuario_encontrado = _context.Tarjeta.FirstOrDefault(x =>
                x.NumeroTarjeta == autorizacion.NumeroTarjeta &&
                x.Pin == autorizacion.Pin
            );

            if (usuario_encontrado == null)
            {
                return await Task.FromResult<AutorizacionResponse>(null);
            }


            string tokenCreado = GenerarToken(usuario_encontrado.IdTarjeta.ToString());

            string refreshTokenCreado = GenerarRefreshToken();

            //return new AutorizacionResponse() { Token = tokenCreado, Resultado = true, Msg = "Ok" };

            return await GuardarHistorialRefreshToken(usuario_encontrado.IdTarjeta, tokenCreado, refreshTokenCreado);
        }

        public async Task<TarjetaResponse> DevolverTarjeta(TarjetaRequest verificar)
        {
            var tarjeta_encontrado = _context.Tarjeta.FirstOrDefault(x =>
                x.NumeroTarjeta == verificar.NumeroTarjeta &&
                x.EsActivo == true
            );

            if (tarjeta_encontrado == null)
            {
                return await Task.FromResult<TarjetaResponse>(null);
            }

            return new TarjetaResponse() { Resultado = true, Msg = "Ok" };
        }

        public async Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest refreshTokenRequest, int idTarjeta)
        {
            var refreshTokenEncontrado = _context.HistorialRefreshTokens.FirstOrDefault(x =>
            x.Token == refreshTokenRequest.TokenExpirado &&
            x.RefreshToken == refreshTokenRequest.RefreshToken &&
            x.IdTarjeta == idTarjeta);

            if (refreshTokenEncontrado == null)
                return new AutorizacionResponse { Resultado = false, Msg = "No existe refreshToken" };

            var refreshTokenCreado = GenerarRefreshToken();
            var tokenCreado = GenerarToken(idTarjeta.ToString());

            return await GuardarHistorialRefreshToken(idTarjeta, tokenCreado, refreshTokenCreado);
        }
    }
}
