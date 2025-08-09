using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace HandsOn.Regras
{
    public static class Mapeamentos
    {
        public static void RegistraMapeamentos(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Repositorio.Entidades.PlanoContas, Dto.PlanoContasDto>()
                .AfterMap((src, dest) =>
                {
                    dest.Codigo = src.CodigoCompleto;

                }).ReverseMap();
                
                cfg.CreateMap<Repositorio.Entidades.Usuario, Dto.UsuarioDto>().ReverseMap();
            });

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
