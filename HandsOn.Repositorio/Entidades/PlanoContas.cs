namespace HandsOn.Repositorio.Entidades
{
    public class PlanoContas
    {
        public long Id { get; set; }
        public int Codigo { get; set; }
        public string CodigoCompleto { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int TipoId { get; set; }
        public long IdPai { get; set; }
        public bool Lancamentos { get; set; }
        public int Nivel { get; set; }
        public int Ordem { get; set; }
    }
}
