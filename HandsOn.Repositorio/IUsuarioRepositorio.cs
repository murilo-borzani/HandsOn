using HandsOn.Repositorio.Entidades;

namespace HandsOn.Repositorio
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> RetornaUsuario(string clientId);
    }
}
