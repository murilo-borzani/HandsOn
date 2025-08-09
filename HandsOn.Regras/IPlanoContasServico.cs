
using HandsOn.Dto;

namespace HandsOn.Regras
{
    public interface IPlanoContasServico
    {
        Task<PlanoContasDto> Criar(PlanoContasDto planoContas, CancellationToken cancellationToken);
        Task<(IEnumerable<PlanoContasDto>, long)> Listar(int pagina, int itensPorPagina, string? termo, CancellationToken cancellationToken);
        Task<Dto.PlanoContasDto?> ProximoCodigo(long idPai, CancellationToken cancellationToken);
        Task<string> Remover(long id, CancellationToken cancellationToken);
    }
}
