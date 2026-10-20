using Atividade03Backend.DTOs;
using Atividade03Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atividade03Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntradaProdutosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public EntradaProdutosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaProdutoResponseDto>>> GetEntradas()
        {
            var entradas = await _context.EntradaProdutos
                .Select(e => new EntradaProdutoResponseDto
                {
                    IdentradaProduto = e.IdentradaProduto,
                    Iditem = e.Iditem,
                    DataEntradaProduto = e.DataEntradaProduto,
                    QuantidadeEntrada = e.QuantidadeEntrada,
                    Unidade = e.Unidade,
                    ValorUnitarioEntrada = e.ValorUnitarioEntrada
                })
                .ToListAsync();

            return Ok(entradas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntradaProdutoResponseDto>> GetEntrada(int id)
        {
            var e = await _context.EntradaProdutos
                .FirstOrDefaultAsync(e => e.IdentradaProduto == id);

            if (e == null) return NotFound("Entrada de produto não encontrada.");

            var response = new EntradaProdutoResponseDto
            {
                IdentradaProduto = e.IdentradaProduto,
                Iditem = e.Iditem,
                DataEntradaProduto = e.DataEntradaProduto,
                QuantidadeEntrada = e.QuantidadeEntrada,
                Unidade = e.Unidade,
                ValorUnitarioEntrada = e.ValorUnitarioEntrada
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEntrada([FromBody] EntradaProdutoCreateUpdateDto dto)
        {
            var itemExiste = await _context.Items.AnyAsync(i => i.Iditem == dto.Iditem);
            if (!itemExiste) return BadRequest("O item informado não existe.");

            var entrada = new EntradaProduto
            {
                Iditem = dto.Iditem,
                DataEntradaProduto = dto.DataEntradaProduto,
                QuantidadeEntrada = dto.QuantidadeEntrada,
                Unidade = dto.Unidade,
                ValorUnitarioEntrada = dto.ValorUnitarioEntrada
            };

            _context.EntradaProdutos.Add(entrada);
            await _context.SaveChangesAsync(); // A trigger do MySQL atualiza o estoque e preço automaticamente

            return CreatedAtAction(nameof(GetEntrada), new { id = entrada.IdentradaProduto }, dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEntrada(int id)
        {
            var entrada = await _context.EntradaProdutos.FindAsync(id);
            if (entrada == null) return NotFound("Entrada não encontrada.");

            _context.EntradaProdutos.Remove(entrada);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}