namespace HandsOn.Api.Models
{
    public class PlanoContasCodigo
    {
        public PlanoContasCodigo(string codigo, long idPai)
        {
            Codigo = codigo;
            IdPai = idPai;
        }

        public string Codigo { get; set; } = string.Empty;
        public Int64 IdPai { get; set; }
    }
}
