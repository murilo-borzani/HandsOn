namespace HandsOn.Dto
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
