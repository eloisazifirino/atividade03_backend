namespace Atividade03Backend.DTOs
{
    public class EnderecoDto
    {
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
    }

    public class PessoaDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateOnly DataNascimento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public EnderecoDto? Endereco { get; set; }
    }

    public class FuncionarioCreateUpdateDto
    {
        public string Cargo { get; set; } = string.Empty;
        public DateOnly DataAdmissao { get; set; }
        public PessoaDto Pessoa { get; set; } = new();
    }

    public class FuncionarioResponseDto
    {
        public int Id { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public DateOnly DataAdmissao { get; set; }
        public PessoaDto Pessoa { get; set; } = new();
    }
}