using Asp.Versioning;
using HandsOn.Api.Models;
using HandsOn.Regras;
using Microsoft.AspNetCore.Mvc;

namespace HandsOn.Api.Controllers.v1
{
    [ApiController]
    [Route("v{version:apiVersion}/seguranca")]
    [ApiVersion("1.0")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SegurancaController : Controller
    {
        readonly ISegurancaServico _segurancaServico;

        public SegurancaController(ISegurancaServico segurancaServico)
        {
            _segurancaServico = segurancaServico ?? throw new ArgumentNullException(nameof(segurancaServico));
        }

        /// <summary>
        /// Gera token de acesso para o cliente, validando as credenciais informadas.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost, Route("token")]
        public async Task<ActionResult<ClienteToken>> Token([FromBody] ClienteApi cliente)
        {
            if (cliente == null)
                return BadRequest(new RetornoErro("Cliente não pode ser nulo"));

            if (string.IsNullOrEmpty(cliente.Tipo))
                return BadRequest(new RetornoErro("Informe grant_type"));

            if (string.IsNullOrEmpty(cliente.Id))
                return BadRequest(new RetornoErro("Informe client_id"));

            if (string.IsNullOrEmpty(cliente.Secret))
                return BadRequest(new RetornoErro("Informe client_secret"));

            var usuario = await _segurancaServico.ValidaCredenciais(cliente.Id, cliente.Secret);

            if (usuario == null)
                return Unauthorized(new RetornoErro("Credenciais inválidas"));

            var tokenResultado = _segurancaServico.GeraToken(usuario);

            if (string.IsNullOrEmpty(tokenResultado.Item1))
                return Unauthorized(new RetornoErro("Credenciais inválidas"));

            return Ok(new Models.ClienteToken
            {
                Tipo = "Bearer",
                Token = tokenResultado.Item1,
                Expiracao = tokenResultado.Item2
            });
        }
    }
}
