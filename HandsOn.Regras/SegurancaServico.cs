using AutoMapper;
using HandsOn.Repositorio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HandsOn.Regras
{
    public class SegurancaServico : ISegurancaServico
    {
        readonly string _issuer;
        readonly string _audience;
        readonly string _chave;
        readonly IUsuarioRepositorio _usuarioRepositorio;
        readonly IMapper _mapper;
        readonly ILogger<SegurancaServico> _logger;

        public SegurancaServico(
            IConfiguration configuration,
            IUsuarioRepositorio usuarioRepositorio,
            IMapper mapper,
            ILogger<SegurancaServico> logger)
        {
            _issuer = configuration.GetValue("Auth:Issuer", "");
            _audience = configuration.GetValue("Auth:Audience", "");
            _chave = configuration.GetValue("Auth:Chave", "");

            if(string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience) || string.IsNullOrEmpty(_chave))
                throw new InvalidOperationException("Configuração jwt inválida");

            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Dto.UsuarioDto?> ValidaCredenciais(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return default;

            try
            {
                var usuario = await _usuarioRepositorio.RetornaUsuario(clientId);

                if (usuario == null || usuario.ClientSecret != clientSecret)
                    return default;

                return _mapper.Map<Dto.UsuarioDto>(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar credenciais do usuário");
            }

            return default;
        }

        public (string,int) GeraToken(Dto.UsuarioDto usuario)
        {
            if (usuario == null)
                return default;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_chave);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome ?? string.Empty),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), (int)(tokenDescriptor.Expires.Value - DateTime.Now).TotalSeconds);
        }
    }
}
