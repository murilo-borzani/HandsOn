namespace HandsOn.Api.Models
{
    public class RetornoErro
    {
        public string Erro { get; set; } = string.Empty;

        public RetornoErro(string erro)
        {
            Erro = erro;
        }
    }
}
