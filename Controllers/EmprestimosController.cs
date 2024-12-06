using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmprestimos.Data;
using SistemaEmprestimos.Models;

namespace SistemaEmprestimos.Controllers
{
    //Define a rota base para este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class EmprestimosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Construtor, onde a conexão com o banco de dados (DbContext) é injetada
        public EmprestimosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint GET para listar todos os empréstimos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimos()
        {
           //Inclui as informações de 'Usuario  e 'Livro nas consultas dos empréstimos 
           return await _context.Emprestimos
           .Include(e => e.Usuario)      
           .Include(e => e.Livro)
           .ToListAsync();
         }

         [HttpGet("{id}")]
         public async Task<ActionResult<Emprestimo>> GetEmprestimo(int id)
         {
            var emprestimo = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

            // Se o emprestimo nao for encontrado, retorna um erro 404
            if (emprestimo ==null)
            {
                return NotFound();
            }

            return emprestimo; //Retorna o empréstimo encontrado
         }

         // Endpoint POST para registrar um novo empréstimo
         [HttpPost]
         public async Task<ActionResult<Emprestimo>> PostEmprestimo(int usuarioId, int livroId)
         {
            var livro = await _context.Livros.FindAsync(livroId);

            //Verifica se o livro existe e se está disponível para empréstimo
            if (livro == null || !livro.Disponivel)
                return BadRequest("Livro não disponível.");

            // Cria o objeto de empréstimo
            var emprestimo = new Emprestimo
            {
                UsuarioId = usuarioId,
                LivroId = livroId,
                DataEmprestimo = DateTime.Now
            };

            livro.Disponivel = false; //Marca o livro como indisponível
            _context.Emprestimos.Add(emprestimo); // Adiciona o emprestimo ao contexto
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados

            return CreatedAtAction("GetEmprestimo", new { id =emprestimo.Id }, emprestimo);
         }

         // Endpoint DELETE para devolver um livro e remover o empréstimo
         [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteEmprestimo(int id)
         {
            var emprestimo = await _context.Emprestimos.FindAsync(id);

            // Se o empréstimo não for encontrado, retorna um erro 404
            if (emprestimo == null)
            {
                return NotFound();
            }

            emprestimo.Livro.Disponivel = true; // Marca o livro como disponível novamente
            _context.Emprestimos.Remove(emprestimo); //Remove o empréstimo
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados

            return NoContent(); // Retorna status 204 indicando sucesso
         }

         }

    }
