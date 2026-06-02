using System;

namespace AluguelQuadrasSUN7
{
    public class OpcaoCadastrar : IOpcaoMenu
    {
        public string Titulo => "Cadastrar Novo Usuário";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRO ===");

            Usuario u = new Usuario();

            // Capturando os dados básicos
            u.Nome = ObterEntradaValida("Nome");
            u.Cpf = ObterEntradaValida("CPF");
            u.Email = ObterEntradaValida("Email");

            // 1. Primeiro captura a senha em texto limpo do console
            string senhaEmTextoPuro = ObterEntradaValida("Senha");

            // 2. Criptografa a senha capturada e joga direto no objeto Usuario
            u.Senha = Security.CriptografarSenha(senhaEmTextoPuro);

            u.Tipo = TipoUsuario.Cliente;

            // Salva o usuário com a senha já protegida
            u.Cadastrar();

            Console.WriteLine("\nUsuário cadastrado com sucesso! Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private string ObterEntradaValida(string nomeCampo)
        {
            string? entrada;
            do
            {
                Console.Write($"{nomeCampo}: ");
                entrada = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(entrada))
                {
                    Console.WriteLine($"{nomeCampo} não pode ser vazio. Tente novamente.");
                }
            } while (string.IsNullOrWhiteSpace(entrada));

            return entrada;
        }
    }
}
