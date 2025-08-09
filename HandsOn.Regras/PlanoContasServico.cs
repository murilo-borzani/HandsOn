using AutoMapper;
using HandsOn.Repositorio;

namespace HandsOn.Regras
{
    public class PlanoContasServico : IPlanoContasServico
    {
        readonly IPlanoContasRepositorio _planoContasRepositorio;
        readonly IMapper _mapper;

        public PlanoContasServico(
            IPlanoContasRepositorio planoContasRepositorio,
            IMapper mapper)
        {
            _planoContasRepositorio = planoContasRepositorio;
            _mapper = mapper;
        }

        public async Task<Dto.PlanoContasDto?> ProximoCodigo(long idPai, CancellationToken cancellationToken)
        {
            return _mapper.Map<Dto.PlanoContasDto?>(
                await _planoContasRepositorio.ProximoCodigo(idPai, cancellationToken));
        }

        public async Task<(IEnumerable<Dto.PlanoContasDto>, long)> Listar(int pagina, int itensPorPagina, string? termo, CancellationToken cancellationToken)
        {
            var (lista, totalRegistros) = await _planoContasRepositorio.Listar(pagina, itensPorPagina, termo, cancellationToken);

            return (_mapper.Map<IEnumerable<Dto.PlanoContasDto>>(lista), totalRegistros);
        }

        public async Task<Dto.PlanoContasDto> Criar(Dto.PlanoContasDto planoContas, CancellationToken cancellationToken)
        {
            #region validações

            planoContas.Erro = string.Empty;

            await CriarValidacoes(planoContas, cancellationToken);

            if (!string.IsNullOrEmpty(planoContas.Erro))
                return planoContas;

            #endregion

            planoContas.Codigo = planoContas.Codigo.LastIndexOf('.') < 0
                ? planoContas.Codigo
                : planoContas.Codigo.Substring(planoContas.Codigo.LastIndexOf('.') + 1);

            planoContas.Id = await _planoContasRepositorio.Criar(
                _mapper.Map<Repositorio.Entidades.PlanoContas>(planoContas), cancellationToken);

            if (planoContas.Id < 1)
                planoContas.Erro = "Conta não criada";

            return planoContas;
        }

        private async Task CriarValidacoes(Dto.PlanoContasDto planoContas, CancellationToken cancellationToken)
        {
            // Os códigos não podem se repetir
            var contaDB = await _planoContasRepositorio.Obter(planoContas.Codigo, cancellationToken);
            if (contaDB != null && contaDB.Id > 0)
            {
                planoContas.Erro = $"Já existe uma conta com o código {planoContas.Codigo}";
                return;
            }

            if (planoContas.IdPai > 0)
            {
                var contaPai = await _planoContasRepositorio.Obter(planoContas.IdPai, cancellationToken);

                if (contaPai == null || contaPai.Id < 1)
                {
                    planoContas.Erro = $"Conta pai ID {planoContas.IdPai} não encontrada";
                    return;
                }
                // A conta que aceita lançamentos não pode ter contas filhas
                // A conta que não aceita lançamentos pode ser pai de outras contas
                else if (contaPai.Lancamentos)
                {
                    planoContas.Erro = $"Conta ID {planoContas.IdPai} não pode ter subcontas pois aceita lançamentos";
                    return;
                }
                // As contas devem obrigatoriamente ser do mesmo tipo do seu pai
                else if (contaPai.TipoId != planoContas.TipoId)
                {
                    planoContas.Erro = $"O tipo da conta não é igual a do pai";
                    return;
                }
                else if (contaPai.CodigoCompleto.Count(x => x == '.') + 1 != planoContas.Codigo.Count(x => x == '.'))
                {
                    var niveis = contaPai.CodigoCompleto.Count(x => x == '.') + 2;
                    planoContas.Erro = $"O código parece inválido. Deve conter {niveis} níveis";
                    return;
                }
                else if (!planoContas.Codigo.StartsWith(contaPai.CodigoCompleto))
                {
                    planoContas.Erro = $"O código parece inválido. Deve iniciar com {contaPai.CodigoCompleto}";
                    return;
                }
            }
        }

        public async Task<string> Remover(long id, CancellationToken cancellationToken)
        {
            var planoContas = await _planoContasRepositorio.Obter(id, cancellationToken);

            if (planoContas == null || planoContas.Id < 1)
                return "Conta não encontrada";

            var possuiFilhos = await _planoContasRepositorio.ContaPossuiFilho(id, cancellationToken);

            if (possuiFilhos)
                return "Conta não pode ser removida pois possui subcontas";

            var resultado = await _planoContasRepositorio.Remover(id, cancellationToken);

            if (resultado < 1)
                return "Conta não removida";

            return string.Empty;
        }
    }
}
