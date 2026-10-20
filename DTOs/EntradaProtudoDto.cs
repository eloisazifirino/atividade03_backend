namespace Atividade03Backend.DTOs
{
    public class EntradaProdutoCreateUpdateDto
    {
        public int Iditem { get; set; }
        public DateOnly DataEntradaProduto { get; set; }
        public int QuantidadeEntrada { get; set; }
        public string Unidade { get; set; } = string.Empty;
        public decimal ValorUnitarioEntrada { get; set; }
    }

    public class EntradaProdutoResponseDto
    {
        public int IdentradaProduto { get; set; }
        public int Iditem { get; set; }
        public DateOnly DataEntradaProduto { get; set; }
        public int QuantidadeEntrada { get; set; }
        public string Unidade { get; set; } = string.Empty;
        public decimal ValorUnitarioEntrada { get; set; }
    }
}