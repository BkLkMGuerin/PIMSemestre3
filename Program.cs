//TODO: CRIAR UM GESTOR QUE POSSA VER RELATÓRIO(LISTA COMPLETA DAS QUADRAS RESERVADAS E FATURAMENTO) COM QUADRAS, PAGAMENTO, AGENDAMENTOS, EDITAR E CADASTRAR QUADRAS
//TODO: FAZER UMA OPÇÃO PARA EDITAR PERFIL CLIENTE
//TODO: AO FINAL DO REGISTRO DE QUADRA OFERECER METODO DE PAGAMENTO PIX
using System;
using System.Collections.Generic;
using AluguelQuadrasSUN7;

namespace AluguelQuadrasSUN7
{
    

    // ==========================================
    // 2. AS TELAS DO SISTEMA (Implementando a Interface)
    // ==========================================

    public class OpcaoCadastrar : IOpcaoMenu
    {
        public string Titulo => "Cadastrar Novo Usuário";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRO ===");
            Usuario u = new Usuario();
            
            // Pegando os dados básicos pra testar
            u.Nome = ObterEntradaValida("Nome");
            u.Cpf = ObterEntradaValida("CPF");
            u.Email = ObterEntradaValida("Email");
            u.Senha = ObterEntradaValida("Senha");
            u.Tipo = TipoUsuario.Cliente;

            u.Cadastrar();
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

    public class OpcaoLogin : IOpcaoMenu
    {
        public string Titulo => "Fazer Login";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== LOGIN ===");
            Console.Write("Email: "); string email = Console.ReadLine() ?? string.Empty;
            Console.Write("Senha: "); string senha = Console.ReadLine() ?? string.Empty;
            Usuario u = new Usuario();
            
            // Se o login der certo, a gente cria o menu de logado na hora!
            if (u.Login(email, senha))
            {
                Console.ReadKey(); // Pausa pra ler a msg de boas-vindas
                
                // Monta as opções do cliente e passa o usuário logado no construtor
                List<IOpcaoMenu> opcoesLogado = new List<IOpcaoMenu>
                {
                    new OpcaoFazerAgendamento(u),
                    new OpcaoCancelarAgendamento()
                };

                MenuPronto menuLogado = new MenuPronto($"Painel do Cliente", opcoesLogado);
                menuLogado.Exibir(); // Roda o submenu e trava aqui até o usuário sair
            }
            else
            {
                Console.ReadKey();
            }
        }
    }

    public class OpcaoFazerAgendamento : IOpcaoMenu
    {
        private Usuario _usuarioLogado; // Guarda quem tá logado
        public string Titulo => "Fazer um Agendamento";

        // Construtor pra receber o usuário que veio da tela de login
        public OpcaoFazerAgendamento(Usuario usuario)
        {
            _usuarioLogado = usuario;
        }

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== NOVO AGENDAMENTO ===");

            // Pra não dar erro de ID inexistente, crio uma quadra e pagamento rápidos aqui no teste
            Quadra q = new Quadra { Nome = "Quadra Teste", Tipo = TipoQuadra.BeachTennis };
            q.CadastrarQuadra();
            
            Pagamento p = new Pagamento { Valor = 90.00m };
            p.RealizarPagamento();

            // Monta o agendamento real puxando o ID do cara logado
            Agendamento a = new Agendamento
            {
                UsuarioId = _usuarioLogado.Id,
                QuadraId = q.Id,
                PagamentoId = p.Id,
                Data = DateTime.Now.AddDays(1),
                HorarioInicio = new TimeSpan(14, 0, 0),
                HorarioFim = new TimeSpan(15, 0, 0)
            };

            a.FazerAgendamento();
            Console.ReadKey();
        }
    }

    public class OpcaoCancelarAgendamento : IOpcaoMenu
    {
        public string Titulo => "Cancelar Agendamento";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== CANCELAMENTO ===");
            Console.Write("Digite o ID do agendamento que deseja cancelar: ");
            
            if(int.TryParse(Console.ReadLine(), out int idAgendamento))
            {
                Agendamento a = new Agendamento { Id = idAgendamento };
                a.CancelarAgendamento();
            }
            Console.ReadKey();
        }
    }

    // ==========================================
    // 3. O PONTO DE PARTIDA DO PROGRAMA
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            // Cria as opções iniciais (antes de logar)
            List<IOpcaoMenu> menuInicial = new List<IOpcaoMenu>
            {
                new OpcaoLogin(),
                new OpcaoCadastrar()
            };

            // Inicia o motor principal
            MenuPronto sistema = new MenuPronto("Aluguel de Quadras SUN7", menuInicial);
            sistema.Exibir();
        }
    }
}