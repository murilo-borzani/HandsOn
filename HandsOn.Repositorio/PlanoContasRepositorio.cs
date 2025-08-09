using HandsOn.Repositorio.Util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HandsOn.Repositorio
{
    public class PlanoContasRepositorio : IPlanoContasRepositorio
    {
        readonly string _sqlConn;

        public PlanoContasRepositorio(IConfiguration configuration)
        {
            _sqlConn = configuration.GetConnectionString("sql") ?? throw new InvalidOperationException();
        }

        public async Task<Entidades.PlanoContas?> ProximoCodigo(long idPai, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                return await conexao.QueryFirstOrDefaultAsyncWithRetry<Entidades.PlanoContas>(
                    "spPlanoContasProximoCodigo",
                    new { idPai },
                    commandType: System.Data.CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<Entidades.PlanoContas?> Obter(long id, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                return await conexao.QueryFirstOrDefaultAsyncWithRetry<Entidades.PlanoContas>(
                    "select * from vPlanoContas (nolock) where id = @id",
                    new { id },
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<Entidades.PlanoContas?> Obter(string codigo, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                return await conexao.QueryFirstOrDefaultAsyncWithRetry<Entidades.PlanoContas>(
                    "select * from vPlanoContas (nolock) where codigoCompleto = @codigo",
                    new { codigo },
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<(IEnumerable<Entidades.PlanoContas>, long)> Listar(int pagina, int itensPorPagina, string? termo, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                var lista = Enumerable.Empty<Entidades.PlanoContas>();
                long totalRegistros = 0;

                using (var query = await conexao.QueryMultipleAsync<Entidades.PlanoContas>(
                    "spPlanoContasLista", new { pagina, itensPorPagina, termo },
                    commandType: System.Data.CommandType.StoredProcedure))
                {
                    totalRegistros = await query.ReadFirstOrDefaultAsync<long>();
                    lista = await query.ReadAsync<Entidades.PlanoContas>();
                }

                return (lista, totalRegistros);
            }
        }

        public async Task<long> Criar(Entidades.PlanoContas planoContas, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();
                return await conexao.ExecuteAsyncWithRetry<long>(
                    @"
INSERT INTO PlanoContas
           (codigo
           ,nome
           ,tipoId
           ,idPai
           ,lancamentos)
     VALUES
           (@codigo
           ,@nome
           ,@tipoId
           ,@idPai
           ,@lancamentos)

SELECT SCOPE_IDENTITY()",
                    planoContas,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<bool> ContaPossuiFilho(long id, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                var resultado = await conexao.ExecuteAsyncWithRetry<int>(
                    "select top 1 id from PlanoContas (nolock) where idPai = @id",
                    new { id },
                    cancellationToken: cancellationToken);

                return resultado > 0;
            }
        }

        public async Task<int> Remover(long id, CancellationToken cancellationToken)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                return await conexao.ExecuteAsyncWithRetry(
                    "delete from PlanoContas where id = @id",
                    new { id },
                    cancellationToken: cancellationToken);
            }
        }
    }
}
