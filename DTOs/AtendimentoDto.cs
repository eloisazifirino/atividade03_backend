namespace Atividade03Backend.DTOs
{
    public class AtendimentoItemRequestDto
    {
        public int Iditem { get; set; }
        public int Idfuncionario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorunitarioEntrada { get; set; }
    }

    public class AtendimentoItemResponseDto
    {
        public int Iditem { get; set; }
        public string DescricaoItem { get; set; } = string.Empty;
        public int Idfuncionario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorunitarioEntrada { get; set; }
    }

    public class AtendimentoCreateUpdateDto
    {
        public int Idpessoa { get; set; }
        public DateOnly DataAtendimento { get; set; }
        public string StatusAtendimento { get; set; } = string.Empty;
        public List<AtendimentoItemRequestDto> Itens { get; set; } = new();
    }

    public class AtendimentoResponseDto
    {
        public int Idatendimento { get; set; }
        public int Idpessoa { get; set; }
        public string NomePessoa { get; set; } = string.Empty;
        public DateOnly DataAtendimento { get; set; }
        public string StatusAtendimento { get; set; } = string.Empty;
        public List<AtendimentoItemResponseDto> Itens { get; set; } = new();
    }
}