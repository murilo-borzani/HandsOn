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


    }
}
