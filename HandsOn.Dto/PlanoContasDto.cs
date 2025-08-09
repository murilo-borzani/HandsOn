namespace HandsOn.Dto
{
    public class PlanoContasDto
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int TipoId { get; set; }
        public long IdPai { get; set; }
        public bool Lancamentos { get; set; }

        public string Erro { get;set; } = string.Empty;
    }
}
