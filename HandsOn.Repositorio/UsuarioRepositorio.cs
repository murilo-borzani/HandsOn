using HandsOn.Repositorio.Util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HandsOn.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        readonly string _sqlConn;

        public UsuarioRepositorio(IConfiguration configuration)
        {
            _sqlConn = configuration.GetConnectionString("sql") ?? throw new InvalidOperationException();
        }

        public async Task<Entidades.Usuario?> RetornaUsuario(string clientId)
        {
            using (var conexao = new SqlConnection(_sqlConn))
            {
                await conexao.OpenAsync();

                return await conexao.QueryFirstOrDefaultAsyncWithRetry<Entidades.Usuario>(
                    "select * from Usuario (nolock) where clientId = @clientId", new { clientId });
            }

        }
    }
}
