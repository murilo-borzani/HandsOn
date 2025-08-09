
using HandsOn.Repositorio.Entidades;

namespace HandsOn.Repositorio
{
    public interface IPlanoContasRepositorio
    {
        Task<bool> ContaPossuiFilho(long id, CancellationToken cancellationToken);
        Task<long> Criar(PlanoContas planoContas, CancellationToken cancellationToken);
        Task<int> Remover(long id, CancellationToken cancellationToken);
        Task<(IEnumerable<PlanoContas>, long)> Listar(int pagina, int itensPorPagina, string? termo, CancellationToken cancellationToken);
        Task<PlanoContas?> Obter(long id, CancellationToken cancellationToken);
        Task<PlanoContas?> Obter(string codigo, CancellationToken cancellationToken);
        Task<Entidades.PlanoContas?> ProximoCodigo(long idPai, CancellationToken cancellationToken);
    }
}
