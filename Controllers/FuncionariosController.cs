using Atividade03Backend.DTOs;
using Atividade03Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atividade03Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionariosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public FuncionariosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // GET: api/Funcionarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FuncionarioResponseDto>>> GetFuncionarios()
        {
            var funcionarios = await _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                    .ThenInclude(p => p.Enderecos)
                .Select(f => new FuncionarioResponseDto
                {
                    Id = f.Idpessoa,
                    Cargo = f.Cargo,
                    DataAdmissao = f.DataAdmissao,
                    Pessoa = new PessoaDto
                    {
                        Nome = f.IdpessoaNavigation.Nome,
                        Cpf = f.IdpessoaNavigation.Cpf.ToString(),
                        DataNascimento = f.IdpessoaNavigation.DataNascimento,
                        Sexo = f.IdpessoaNavigation.Sexo,
                        Email = f.IdpessoaNavigation.Email,
                        Telefone = f.IdpessoaNavigation.Telefone.ToString(),
                        Endereco = f.IdpessoaNavigation.Enderecos.FirstOrDefault() != null ? new EnderecoDto
                        {
                            Cep = f.IdpessoaNavigation.Enderecos.First().Cep,
                            Numero = f.IdpessoaNavigation.Enderecos.First().Numero,
                            Complemento = f.IdpessoaNavigation.Enderecos.First().Complemento
                        } : null
                    }
                })
                .ToListAsync();

            return Ok(funcionarios);
        }

        // GET: api/Funcionarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioResponseDto>> GetFuncionario(int id)
        {
            var f = await _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                    .ThenInclude(p => p.Enderecos)
                .FirstOrDefaultAsync(f => f.Idpessoa == id);

            if (f == null) return NotFound("Funcionário não encontrado.");

            var enderecoObj = f.IdpessoaNavigation.Enderecos.FirstOrDefault();

            var response = new FuncionarioResponseDto
            {
                Id = f.Idpessoa,
                Cargo = f.Cargo,
                DataAdmissao = f.DataAdmissao,
                Pessoa = new PessoaDto
                {
                    Nome = f.IdpessoaNavigation.Nome,
                    Cpf = f.IdpessoaNavigation.Cpf.ToString(),
                    DataNascimento = f.IdpessoaNavigation.DataNascimento,
                    Sexo = f.IdpessoaNavigation.Sexo,
                    Email = f.IdpessoaNavigation.Email,
                    Telefone = f.IdpessoaNavigation.Telefone.ToString(),
                    Endereco = enderecoObj != null ? new EnderecoDto
                    {
                        Cep = enderecoObj.Cep,
                        Numero = enderecoObj.Numero,
                        Complemento = enderecoObj.Complemento
                    } : null
                }
            };

            return Ok(response);
        }

        // POST: api/Funcionarios
        [HttpPost]
        public async Task<ActionResult> CreateFuncionario([FromBody] FuncionarioCreateUpdateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cpfLimpo = new string(dto.Pessoa.Cpf.Where(char.IsDigit).ToArray());
                var telLimpo = new string(dto.Pessoa.Telefone.Where(char.IsDigit).ToArray());

                var pessoa = new Pessoa
                {
                    Nome = dto.Pessoa.Nome,
                    Cpf = ulong.TryParse(cpfLimpo, out var cpfVal) ? cpfVal : 0,
                    DataNascimento = dto.Pessoa.DataNascimento,
                    Sexo = dto.Pessoa.Sexo,
                    Email = dto.Pessoa.Email,
                    Telefone = ulong.TryParse(telLimpo, out var telVal) ? telVal : 0,
                    AtivoPessoa = "S"
                };

                _context.Pessoas.Add(pessoa);
                await _context.SaveChangesAsync();

                if (dto.Pessoa.Endereco != null)
                {
                    var endereco = new Endereco
                    {
                        Idpessoa = pessoa.Idpessoa,
                        Cep = dto.Pessoa.Endereco.Cep,
                        Numero = dto.Pessoa.Endereco.Numero,
                        Complemento = dto.Pessoa.Endereco.Complemento ?? string.Empty
                    };
                    _context.Enderecos.Add(endereco);
                }

                var funcionario = new Funcionario
                {
                    Idpessoa = pessoa.Idpessoa,
                    Cargo = dto.Cargo,
                    DataAdmissao = dto.DataAdmissao
                };

                _context.Funcionarios.Add(funcionario);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.Idpessoa }, dto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Erro ao cadastrar funcionário: {ex.Message}");
            }
        }

        // DELETE: api/Funcionarios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound("Funcionário não encontrado.");

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}