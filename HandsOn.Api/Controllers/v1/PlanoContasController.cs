using Asp.Versioning;
using HandsOn.Api.Models;
using HandsOn.Regras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandsOn.Api.Controllers.v1;

[ApiController]
[Route("v{version:apiVersion}/planocontas")]
[ApiVersion("1.0")]
[Authorize]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PlanoContasController : ControllerBase
{
    readonly IPlanoContasServico _planoContasServico;

    public PlanoContasController(
        IPlanoContasServico planoContasServico)
    {
        _planoContasServico = planoContasServico ?? throw new ArgumentNullException(nameof(planoContasServico));

    }

    /// <summary>
    /// Retorna o próximo código para o plano de contas, dado o identificador do nó pai.
    /// Se o nível chegou ao limite, retorna o próximo disponível no nível acima
    /// </summary>
    /// <param name="pai">Identificador do nó pai</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Código novo e identificador pai</returns>
    [HttpGet, Route("proximocodigo/{pai:int}")]
    [ProducesResponseType(typeof(Dto.PlanoContasDto), 200)]
    [ProducesResponseType(typeof(RetornoErro), 400)]
    [ProducesResponseType(typeof(RetornoErro), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dto.PlanoContasDto>> ProximoCodigo(int? pai, CancellationToken cancellationToken)
    {
        if (pai == null || pai < 0)
            return BadRequest(new RetornoErro("O identificador pai deve ser maior ou igual a zero"));

        var planoContas = await _planoContasServico.ProximoCodigo(pai.Value, cancellationToken);

        if (planoContas == null || string.IsNullOrEmpty(planoContas.Codigo))
            return NotFound(new RetornoErro("Não foi possível gerar o próximo código para o plano de contas"));

        return Ok(new Models.PlanoContasCodigo(planoContas.Codigo, planoContas.IdPai));
    }

    /// <summary>
    /// Lista os planos de contas com paginação e filtro por termo.
    /// </summary>
    /// <param name="pagina">Número da página atual</param>
    /// <param name="itensPorPagina">Quantidade de registros para retornar</param>
    /// <param name="termo">Palavra-chave para filtrar a lista</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet, Route("lista/{pagina:int}/{itensPorPagina:int}/{termo?}")]
    [ProducesResponseType(typeof(IEnumerable<Dto.PlanoContasDto>), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<Dto.PlanoContasDto>>> Lista(int pagina, int itensPorPagina, string? termo, CancellationToken cancellationToken)
    {
        var (lista, totalRegistros) = await _planoContasServico.Listar(pagina, itensPorPagina, termo, cancellationToken);

        if (totalRegistros == 0)
            return NoContent();

        return Ok(new Models.PlanoContasLista(lista, totalRegistros));
    }

    /// <summary>
    /// Cria um plano de conta
    /// </summary>
    /// <param name="planoConta"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, Route("")]
    [ProducesResponseType(typeof(Dto.PlanoContasDto), 200)]
    [ProducesResponseType(typeof(RetornoErro), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dto.PlanoContasDto>> CriarPlanoConta([FromBody] Dto.PlanoContasDto planoConta, CancellationToken cancellationToken)
    {
        if (planoConta == null)
            return BadRequest(new RetornoErro("O plano de contas não pode ser nulo"));

        if (string.IsNullOrEmpty(planoConta.Codigo) || planoConta.IdPai < 0)
            return BadRequest(new RetornoErro("O código e o identificador pai devem ser informados"));

        planoConta = await _planoContasServico.Criar(planoConta, cancellationToken);

        if (!string.IsNullOrEmpty(planoConta.Erro))
            return BadRequest(new RetornoErro(planoConta.Erro));

        return Ok(planoConta);
    }

    /// <summary>
    /// Remove um plano de conta pelo identificador
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete, Route("{id:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(RetornoErro), 400)]
    [ProducesResponseType(typeof(RetornoErro), 404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RemoverPlanoConta(long id, CancellationToken cancellationToken)
    {
        if (id < 1)
            return BadRequest(new RetornoErro("O identificador da conta deve ser maior que zero"));

        var resultado = await _planoContasServico.Remover(id, cancellationToken);

        if (!string.IsNullOrEmpty(resultado))
        {
            if (resultado.Contains("não encontrada"))
                return NotFound(new RetornoErro(resultado));
            else
                return BadRequest(new RetornoErro(resultado));
        }

        return Ok();
    }

}

