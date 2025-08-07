using HandsOn.Dto;

namespace HandsOn.Regras
{
    public interface ISegurancaServico
    {
        (string,int) GeraToken(Dto.UsuarioDto usuario);
        Task<UsuarioDto?> ValidaCredenciais(string clientId, string clientSecret);
    }
}
