using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmprestimos.Data;
using SistemaEmprestimos.Models;

namespace SistemaEmprestimos.Controllers
{
    // Define a rota base para este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Construtor, onde a conexão com o banco de dados (DbContext) é injetada
        public LivrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint GET para listar todos os livros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livro>>> GetLivros()
        {
            return await _context.Livros.ToListAsync(); // Retorna todos os livros no banco
        }

        // Endpoint GET para obter um livro específico pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Livro>> GetLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            // Se o livro não for encontrado, retorna um erro 404
            if (livro == null)
            {
                return NotFound();
            }

            return livro; // Retorna o livro encontrado
        }

        // Endpoint POST para criar um novo livro
        [HttpPost]
        public async Task<ActionResult<Livro>> PostLivro(Livro livro)
        {
            _context.Livros.Add(livro); // Adiciona o livro ao contexto
            await _context.SaveChangesAsync(); // Salva no banco de dados

            // Retorna o livro criado com o código 201 (Created)
            return CreatedAtAction("GetLivro", new { id = livro.Id }, livro);
        }

        // Endpoint PUT para atualizar as informações de um livro
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, Livro livro)
        {
            if (id != livro.Id) // Verifica se o ID corresponde
            {
                return BadRequest(); // Retorna erro 400 se não coincidir
            }

            _context.Entry(livro).State = EntityState.Modified; // Marca para atualização

            try
            {
                await _context.SaveChangesAsync(); // Salva as alterações
            }
            catch (DbUpdateConcurrencyException) // Trata erros de concorrência
            {
                if (!LivroExists(id)) // Verifica se o livro existe
                {
                    return NotFound(); // Retorna erro 404 se não encontrar
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Retorna um status 204 indicando sucesso
        }

        // Endpoint DELETE para excluir um livro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            // Se o livro não for encontrado, retorna erro 404
            if (livro == null)
            {
                return NotFound();
            }

            _context.Livros.Remove(livro); // Remove o livro do banco
            await _context.SaveChangesAsync(); // Salva as mudanças

            return NoContent(); // Retorna status 204 indicando sucesso
        }

        // Método auxiliar para verificar se o livro existe no banco
        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id); // Retorna true se o livro existir, false caso contrário
        }
    }
}
