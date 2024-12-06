using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmprestimos.Data;
using SistemaEmprestimos.Models;

namespace SistemaEmprestimos.Controllers
{
    // Define a rota base para este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Construtor, onde a conexão com o banco de dados (DbContext) é injetada
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint GET para listar todos os usuários
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            // Retorna todos os usuários do banco de dados
            return await _context.Usuarios.ToListAsync();
        }

        // Endpoint GET para obter um usuário específico pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            // Se o usuário não for encontrado, retorna um erro 404 (Not Found)
            if (usuario == null)
            {
                return NotFound();
            }

            return usuario; // Retorna o usuário encontrado
        }

        // Endpoint POST para criar um novo usuário
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario); // Adiciona o usuário ao contexto (não está no banco ainda)
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados

            // Retorna o usuário criado com o código de status 201 (Created)
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // Endpoint PUT para atualizar as informações de um usuário
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) // Verifica se o ID informado no URL corresponde ao ID do usuário
            {
                return BadRequest(); // Retorna um erro 400 se os IDs não coincidirem
            }

            _context.Entry(usuario).State = EntityState.Modified; // Marca o usuário para atualização

            try
            {
                await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados
            }
            catch (DbUpdateConcurrencyException) // Se houver um erro de concorrência, trata o erro
            {
                if (!UsuarioExists(id)) // Verifica se o usuário existe no banco
                {
                    return NotFound(); // Retorna um erro 404 se o usuário não for encontrado
                }
                else
                {
                    throw; // Lança o erro novamente se for outro tipo de falha
                }
            }

            return NoContent(); // Retorna um status 204 (sem conteúdo) indicando sucesso
        }

        // Endpoint DELETE para excluir um usuário
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            // Se o usuário não for encontrado, retorna um erro 404
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario); // Remove o usuário do banco de dados
            await _context.SaveChangesAsync(); // Salva as mudanças no banco

            return NoContent(); // Retorna um status 204 indicando que a exclusão foi bem-sucedida
        }

        // Método auxiliar para verificar se o usuário existe no banco
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id); // Retorna true se o usuário existir, false caso contrário
        }
    }
}
