using Atividade03Backend.DTOs;
using Atividade03Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atividade03Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public AtendimentosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtendimentoResponseDto>>> GetAtendimentos()
        {
            var atendimentos = await _context.Atendimentos
                .Include(a => a.IdpessoaNavigation)
                .Include(a => a.Atendimentoitems)
                    .ThenInclude(ai => ai.IditemNavigation)
                .Select(a => new AtendimentoResponseDto
                {
                    Idatendimento = a.Idatendimento,
                    Idpessoa = a.Idpessoa,
                    NomePessoa = a.IdpessoaNavigation.Nome,
                    DataAtendimento = a.DataAtendimento,
                    StatusAtendimento = a.StatusAtendimento,
                    Itens = a.Atendimentoitems.Select(ai => new AtendimentoItemResponseDto
                    {
                        Iditem = ai.Iditem,
                        DescricaoItem = ai.IditemNavigation.DescricaoItem,
                        Idfuncionario = ai.Idfuncionario,
                        QuantidadeItens = ai.QuantidadeItens,
                        ValorunitarioEntrada = ai.ValorunitarioEntrada
                    }).ToList()
                })
                .ToListAsync();

            return Ok(atendimentos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AtendimentoResponseDto>> GetAtendimento(int id)
        {
            var a = await _context.Atendimentos
                .Include(a => a.IdpessoaNavigation)
                .Include(a => a.Atendimentoitems)
                    .ThenInclude(ai => ai.IditemNavigation)
                .FirstOrDefaultAsync(a => a.Idatendimento == id);

            if (a == null) return NotFound("Atendimento não encontrado.");

            var response = new AtendimentoResponseDto
            {
                Idatendimento = a.Idatendimento,
                Idpessoa = a.Idpessoa,
                NomePessoa = a.IdpessoaNavigation.Nome,
                DataAtendimento = a.DataAtendimento,
                StatusAtendimento = a.StatusAtendimento,
                Itens = a.Atendimentoitems.Select(ai => new AtendimentoItemResponseDto
                {
                    Iditem = ai.Iditem,
                    DescricaoItem = ai.IditemNavigation.DescricaoItem,
                    Idfuncionario = ai.Idfuncionario,
                    QuantidadeItens = ai.QuantidadeItens,
                    ValorunitarioEntrada = ai.ValorunitarioEntrada
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAtendimento([FromBody] AtendimentoCreateUpdateDto dto)
        {
            var pessoaExiste = await _context.Pessoas.AnyAsync(p => p.Idpessoa == dto.Idpessoa);
            if (!pessoaExiste) return BadRequest("Pessoa informada não existe.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var atendimento = new Atendimento
                {
                    Idpessoa = dto.Idpessoa,
                    DataAtendimento = dto.DataAtendimento,
                    StatusAtendimento = dto.StatusAtendimento
                };

                _context.Atendimentos.Add(atendimento);
                await _context.SaveChangesAsync();

                foreach (var itemDto in dto.Itens)
                {
                    var itemExiste = await _context.Items.AnyAsync(i => i.Iditem == itemDto.Iditem);
                    var funcExiste = await _context.Funcionarios.AnyAsync(f => f.Idpessoa == itemDto.Idfuncionario);

                    if (!itemExiste || !funcExiste)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Item ({itemDto.Iditem}) ou Funcionário ({itemDto.Idfuncionario}) não encontrado.");
                    }

                    var atendimentoItem = new Atendimentoitem
                    {
                        Idatendimento = atendimento.Idatendimento,
                        Iditem = itemDto.Iditem,
                        Idfuncionario = itemDto.Idfuncionario,
                        QuantidadeItens = itemDto.QuantidadeItens,
                        ValorunitarioEntrada = itemDto.ValorunitarioEntrada
                    };

                    _context.Atendimentoitems.Add(atendimentoItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetAtendimento), new { id = atendimento.Idatendimento }, dto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Erro ao registar atendimento: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAtendimento(int id)
        {
            var atendimento = await _context.Atendimentos.FindAsync(id);
            if (atendimento == null) return NotFound("Atendimento não encontrado.");

            _context.Atendimentos.Remove(atendimento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}