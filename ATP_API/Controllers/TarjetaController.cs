using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ATP_API.Models.Custom;
using ATP_API.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace ATP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaController : ControllerBase
    {
        private readonly IAutorizacionService _autorizacionService;

        public TarjetaController(IAutorizacionService autorizacionService)
        {
            _autorizacionService = autorizacionService;
        }

        [HttpPost]
        [Route("VerificarTarjeta")]
        public async Task<IActionResult> VerificarTarjeta([FromBody] TarjetaRequest verificar)
        {
            var resultado = await _autorizacionService.DevolverTarjeta(verificar);
            if (resultado == null)
                return BadRequest(new TarjetaResponse { Resultado = false, Msg = "Tarjeta no encontrada o bloqueada" });

            return Ok(resultado);
        }

        [HttpPost]
        [Route("Autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] AutorizacionRequest autorizacion)
        {
            var resultado_autorizacion = await _autorizacionService.DevolverToken(autorizacion);
            if (resultado_autorizacion == null)
                return BadRequest(new AutorizacionResponse { Token = null, RefreshToken = null, Resultado = false, Msg = "Ingresó un Pin Erroneo." });
            //return Unauthorized();

            return Ok(resultado_autorizacion);
        }

        [HttpPost]
        [Route("ObtenerRefreshToken")]
        public async Task<IActionResult> ObtenerRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExpiradoSupuestamente = tokenHandler.ReadJwtToken(request.TokenExpirado);

            if (tokenExpiradoSupuestamente.ValidTo > DateTime.UtcNow)
                return BadRequest(new AutorizacionResponse { Resultado = false, Msg = "Token no ha expirado" });

            string idTarjeta = tokenExpiradoSupuestamente.Claims.First(x =>
                x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

            var autorizacionResponse = await _autorizacionService.DevolverRefreshToken(request, int.Parse(idTarjeta));

            if (autorizacionResponse.Resultado)
                return Ok(autorizacionResponse);
            else
                return BadRequest(autorizacionResponse);
        }

        [HttpPost]
        [Route("BloquearTarjeta")]
        public async Task<IActionResult> BloquearTarjeta([FromBody] TarjetaRequest bloquear)
        {
            var resultado = await _autorizacionService.BloquearTarjeta(bloquear);
            return Ok(resultado);
        }

        [Authorize]
        [HttpPost]
        [Route("RetirarDinero")]
        public async Task<IActionResult> RetirarDinero([FromBody] AutorizacionRequest retirar)
        {
            var resultado = await _autorizacionService.RetirarDinero(retirar);
            return Ok(resultado);
        }
    }
}
