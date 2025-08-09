using HandsOn.Dto;

namespace HandsOn.Api.Models
{
    /// <summary>
    /// Objeto de retorno para listas de planos de contas
    /// </summary>
    public class PlanoContasLista
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itens"></param>
        /// <param name="totalRegistros"></param>
        public PlanoContasLista(IEnumerable<PlanoContasDto> itens, Int64 totalRegistros)
        {
            Itens = itens;
            TotalRegistros = totalRegistros;
        }

        /// <summary>
        /// Lista de planos de contas
        /// </summary>
        public IEnumerable<Dto.PlanoContasDto> Itens { get; set; } = [];

        /// <summary>
        /// Total de registros na consulta
        /// </summary>
        public Int64 TotalRegistros { get; set; }
    }
}
