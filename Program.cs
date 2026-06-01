
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
                
                List<IOpcaoMenu> opcoesLogado;
                string tituloPainel;

                if (u.Tipo == TipoUsuario.Gestor)
                {
                    tituloPainel = "Painel do Gestor";
                    opcoesLogado = new List<IOpcaoMenu>
                    {
                        new OpcaoCadastrarQuadra(),
                        new OpcaoEditarQuadra(),
                        new OpcaoVisualizarRelatorio()
                    };
                }
                else
                {
                    tituloPainel = "Painel do Cliente";
                    opcoesLogado = new List<IOpcaoMenu>
                    {
                        new OpcaoFazerAgendamento(u),
                        new OpcaoCancelarAgendamento(),
                        new OpcaoEditarPerfilCliente(u)
                    };
                }

                MenuPronto menuLogado = new MenuPronto(tituloPainel, opcoesLogado);
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

            string arquivoQuadras = "quadras.txt";
            if (!File.Exists(arquivoQuadras))
            {
                Console.WriteLine("Nenhuma quadra cadastrada no sistema. Não é possível fazer agendamentos.");
                Console.ReadKey();
                return;
            }

            string[] linhasQuadras = File.ReadAllLines(arquivoQuadras);
            var quadrasDisponiveis = new List<(int id, string nome, TipoQuadra tipo)>();

            Console.WriteLine("\nQuadras Disponíveis:");
            Console.WriteLine("ID | Nome | Tipo");
            Console.WriteLine("----------------");
            foreach (var linha in linhasQuadras)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;
                string[] dados = linha.Split('|');
                if (dados.Length >= 4)
                {
                    int idQ = int.Parse(dados[0]);
                    string nomeQ = dados[1];
                    TipoQuadra tipoQ = (TipoQuadra)Enum.Parse(typeof(TipoQuadra), dados[2]);
                    bool disponivelQ = bool.Parse(dados[3]);

                    if (disponivelQ)
                    {
                        quadrasDisponiveis.Add((idQ, nomeQ, tipoQ));
                        Console.WriteLine($"{idQ} | {nomeQ} | {tipoQ}");
                    }
                }
            }
            Console.WriteLine("----------------");

            if (quadrasDisponiveis.Count == 0)
            {
                Console.WriteLine("Nenhuma quadra está disponível no momento.");
                Console.ReadKey();
                return;
            }

            Console.Write("\nDigite o ID da quadra que deseja reservar: ");
            if (!int.TryParse(Console.ReadLine(), out int quadraId) || !quadrasDisponiveis.Exists(q => q.id == quadraId))
            {
                Console.WriteLine("ID de quadra inválido ou indisponível.");
                Console.ReadKey();
                return;
            }

            // --- Validação da Data ---
            DateTime dataReserva;
            while (true)
            {
                Console.Write("Digite a data (DD/MM/AAAA): ");
                string dataStr = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(dataStr))
                {
                    Console.WriteLine("  A data não pode ser vazia. Tente novamente.");
                    continue;
                }
                if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataReserva))
                {
                    Console.WriteLine("  Formato inválido. Use DD/MM/AAAA. Tente novamente.");
                    continue;
                }
                if (dataReserva.Date < DateTime.Today)
                {
                    Console.WriteLine("  Não é possível agendar para uma data no passado. Tente novamente.");
                    continue;
                }
                break;
            }

            // --- Validação do Horário de Início ---
            TimeSpan horarioInicio;
            while (true)
            {
                Console.Write("Horário de Início (HH:MM): ");
                string hrIniStr = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(hrIniStr))
                {
                    Console.WriteLine("  O horário de início não pode ser vazio. Tente novamente.");
                    continue;
                }
                if (!TimeSpan.TryParseExact(hrIniStr, @"hh\:mm", null, out horarioInicio))
                {
                    Console.WriteLine("  Formato inválido. Use HH:MM (ex: 14:00). Tente novamente.");
                    continue;
                }
                // Verifica se o agendamento é hoje e o horário já passou
                if (dataReserva.Date == DateTime.Today && horarioInicio <= DateTime.Now.TimeOfDay)
                {
                    Console.WriteLine("  Horário já passou para hoje. Escolha um horário futuro.");
                    continue;
                }
                break;
            }

            // --- Validação do Horário de Fim ---
            TimeSpan horarioFim;
            while (true)
            {
                Console.Write("Horário de Fim (HH:MM): ");
                string hrFimStr = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(hrFimStr))
                {
                    Console.WriteLine("  O horário de fim não pode ser vazio. Tente novamente.");
                    continue;
                }
                if (!TimeSpan.TryParseExact(hrFimStr, @"hh\:mm", null, out horarioFim))
                {
                    Console.WriteLine("  Formato inválido. Use HH:MM (ex: 15:00). Tente novamente.");
                    continue;
                }
                if (horarioFim <= horarioInicio)
                {
                    Console.WriteLine("  O horário de fim deve ser maior que o de início. Tente novamente.");
                    continue;
                }
                break;
            }

            // Calcula o valor do pagamento baseado nas horas de reserva (custo de R$ 90,00 por hora)
            double totalHoras = (horarioFim - horarioInicio).TotalHours;
            decimal valorAgendamento = totalHoras > 0 ? (decimal)totalHoras * 90.00m : 90.00m;

            // Cria o pagamento
            Pagamento p = new Pagamento { Valor = valorAgendamento };
            p.RealizarPagamento();

            // Monta o agendamento real puxando o ID do cara logado
            Agendamento a = new Agendamento
            {
                UsuarioId = _usuarioLogado.Id,
                QuadraId = quadraId,
                PagamentoId = p.Id,
                Data = dataReserva,
                HorarioInicio = horarioInicio,
                HorarioFim = horarioFim
            };

            a.FazerAgendamento();

            Console.WriteLine("\n==============================");
            Console.WriteLine("1 - Sim, realizar pagamento PIX agora");
            Console.WriteLine("2 - Não, pagar mais tarde");
            Console.Write("Escolha uma opção: ");
            string opcaoPg = Console.ReadLine() ?? string.Empty;

            if (opcaoPg == "1")
            {
                p.GerarQRCodePIX();
                Console.WriteLine("\nPressione qualquer tecla após realizar o pagamento no app do banco...");
                Console.ReadKey();
                p.ConfirmarPagamento();
            }
            else
            {
                Console.WriteLine("\nAgendamento concluído! Lembre-se de pagar o Pix antes de utilizar a quadra.");
            }

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

    public class OpcaoCadastrarQuadra : IOpcaoMenu
    {
        public string Titulo => "Cadastrar Nova Quadra";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR NOVA QUADRA ===");
            Quadra q = new Quadra();
            
            Console.Write("Nome da Quadra: ");
            string nome;
            do
            {
                nome = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nome))
                {
                    Console.Write("Nome inválido! Digite novamente: ");
                }
            } while (string.IsNullOrWhiteSpace(nome));
            q.Nome = nome;

            Console.WriteLine("Tipo de Quadra:");
            Console.WriteLine("1 - Volei");
            Console.WriteLine("2 - BeachTennis");
            Console.Write("Escolha uma opção: ");
            string tipoEscolha = Console.ReadLine() ?? string.Empty;
            q.Tipo = tipoEscolha == "2" ? TipoQuadra.BeachTennis : TipoQuadra.Volei;

            q.CadastrarQuadra();

            Console.Write("\nDeseja cadastrar o endereço para esta quadra? (S/N): ");
            string respostaEnd = Console.ReadLine()?.Trim().ToUpper() ?? string.Empty;
            if (respostaEnd == "S")
            {
                Endereco e = new Endereco();
                e.QuadraId = q.Id;
                
                Console.Write("Rua: ");
                e.Rua = Console.ReadLine() ?? string.Empty;
                
                Console.Write("Número: ");
                if (int.TryParse(Console.ReadLine(), out int num))
                {
                    e.Numero = num;
                }
                
                Console.Write("Bairro: ");
                e.Bairro = Console.ReadLine() ?? string.Empty;
                
                Console.Write("Cidade: ");
                e.Cidade = Console.ReadLine() ?? string.Empty;

                e.CadastrarEndereco();
            }

            Console.ReadKey();
        }
    }

    public class OpcaoEditarQuadra : IOpcaoMenu
    {
        public string Titulo => "Editar Quadra Existente";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== EDITAR QUADRA ===");

            string arquivoQuadras = "quadras.txt";
            if (!File.Exists(arquivoQuadras))
            {
                Console.WriteLine("Nenhuma quadra cadastrada no sistema.");
                Console.ReadKey();
                return;
            }

            // Exibir lista de quadras para facilitar
            string[] linhas = File.ReadAllLines(arquivoQuadras);
            Console.WriteLine("ID | Nome | Tipo | Disponível");
            Console.WriteLine("----------------------------");
            foreach (var linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;
                string[] dados = linha.Split('|');
                if (dados.Length >= 4)
                {
                    Console.WriteLine($"{dados[0]} | {dados[1]} | {dados[2]} | {dados[3]}");
                }
            }
            Console.WriteLine("----------------------------");

            Console.Write("Digite o ID da quadra que deseja editar: ");
            if (int.TryParse(Console.ReadLine(), out int idQuadra))
            {
                // Encontrar a quadra
                bool encontrada = false;
                Quadra q = new Quadra { Id = idQuadra };
                foreach (var linha in linhas)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    string[] dados = linha.Split('|');
                    if (int.TryParse(dados[0], out int idArq) && idArq == idQuadra)
                    {
                        encontrada = true;
                        q.Nome = dados[1];
                        q.Tipo = (TipoQuadra)Enum.Parse(typeof(TipoQuadra), dados[2]);
                        q.Disponivel = bool.Parse(dados[3]);
                        break;
                    }
                }

                if (!encontrada)
                {
                    Console.WriteLine("Quadra não encontrada.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nEditando Quadra '{q.Nome}' (Tipo: {q.Tipo})");
                Console.Write("Novo Nome (deixe vazio para manter atual): ");
                string novoNome = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(novoNome))
                {
                    q.Nome = novoNome;
                }

                Console.WriteLine("Alterar Tipo da Quadra? (S/N): ");
                string alterarTipo = Console.ReadLine()?.Trim().ToUpper() ?? string.Empty;
                if (alterarTipo == "S")
                {
                    Console.WriteLine("1 - Volei");
                    Console.WriteLine("2 - BeachTennis");
                    Console.Write("Escolha uma opção: ");
                    string tipoEscolha = Console.ReadLine() ?? string.Empty;
                    q.Tipo = tipoEscolha == "2" ? TipoQuadra.BeachTennis : TipoQuadra.Volei;
                }

                Console.WriteLine("Alterar Disponibilidade? (1 - Disponível, 2 - Indisponível, Enter para manter): ");
                string dispEscolha = Console.ReadLine()?.Trim() ?? string.Empty;
                if (dispEscolha == "1") q.Disponivel = true;
                else if (dispEscolha == "2") q.Disponivel = false;

                q.AtualizarQuadra();
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
            Console.ReadKey();
        }
    }

    public class OpcaoVisualizarRelatorio : IOpcaoMenu
    {
        public string Titulo => "Visualizar Relatórios (Reservas e Faturamento)";

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIO DO GESTOR ===");

            string arquivoAgendamentos = "agendamentos.txt";
            string arquivoQuadras = "quadras.txt";
            string arquivoUsuarios = "usuarios.txt";
            string arquivoPagamentos = "pagamentos.txt";

            // Dicionários para cruzar dados e exibir relatórios ricos
            var dictQuadras = new Dictionary<int, string>();
            if (File.Exists(arquivoQuadras))
            {
                foreach (var linha in File.ReadAllLines(arquivoQuadras))
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    var dados = linha.Split('|');
                    if (dados.Length > 1 && int.TryParse(dados[0], out int id))
                        dictQuadras[id] = dados[1];
                }
            }

            var dictUsuarios = new Dictionary<int, string>();
            if (File.Exists(arquivoUsuarios))
            {
                foreach (var linha in File.ReadAllLines(arquivoUsuarios))
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    var dados = linha.Split('|');
                    if (dados.Length > 1 && int.TryParse(dados[0], out int id))
                        dictUsuarios[id] = dados[1];
                }
            }

            var dictPagamentos = new Dictionary<int, (decimal valor, string status)>();
            decimal faturamentoTotal = 0m;
            if (File.Exists(arquivoPagamentos))
            {
                foreach (var linha in File.ReadAllLines(arquivoPagamentos))
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    var dados = linha.Split('|');
                    if (dados.Length > 2 && int.TryParse(dados[0], out int id))
                    {
                        decimal.TryParse(dados[1], out decimal valor);
                        string status = dados[2];
                        dictPagamentos[id] = (valor, status);

                        if (status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                        {
                            faturamentoTotal += valor;
                        }
                    }
                }
            }

            Console.WriteLine("\n--- QUADRAS RESERVADAS ---");
            if (File.Exists(arquivoAgendamentos))
            {
                string[] linhasAg = File.ReadAllLines(arquivoAgendamentos);
                bool temReservas = false;

                foreach (var linha in linhasAg)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    var dados = linha.Split('|');
                    if (dados.Length >= 8)
                    {
                        int.TryParse(dados[0], out int idAg);
                        int.TryParse(dados[1], out int idUs);
                        int.TryParse(dados[2], out int idQd);
                        int.TryParse(dados[3], out int idPg);
                        string hrInicio = dados[4];
                        string hrFim = dados[5];
                        string data = dados[6];
                        string status = dados[7].Split(' ')[0]; // ignora sufixos como "(Cancelado)"

                        // Exibe apenas agendamentos ativos (Reservado ou Concluido)
                        if (!status.Equals("Reservado", StringComparison.OrdinalIgnoreCase) &&
                            !status.Equals("Concluido", StringComparison.OrdinalIgnoreCase))
                            continue;

                        temReservas = true;

                        string nomeUs = dictUsuarios.ContainsKey(idUs) ? dictUsuarios[idUs] : $"Usuário ID {idUs}";
                        string nomeQd = dictQuadras.ContainsKey(idQd) ? dictQuadras[idQd] : $"Quadra ID {idQd}";
                        
                        string infoPg = "N/A";
                        if (dictPagamentos.ContainsKey(idPg))
                        {
                            infoPg = $"R$ {dictPagamentos[idPg].valor:F2} ({dictPagamentos[idPg].status})";
                        }

                        Console.WriteLine($"Reserva ID: {idAg} | Data: {data} ({hrInicio} - {hrFim})");
                        Console.WriteLine($"  Quadra: {nomeQd} | Cliente: {nomeUs}");
                        Console.WriteLine($"  Status: {status} | Pagamento: {infoPg}");
                        Console.WriteLine("  ------------------------------------------------");
                    }
                }

                if (!temReservas)
                {
                    Console.WriteLine("Nenhum agendamento registrado.");
                }
            }
            else
            {
                Console.WriteLine("Nenhum agendamento registrado.");
            }

            Console.WriteLine("\n=== FATURAMENTO TOTAL ===");
            Console.WriteLine($"Faturamento Aprovado: R$ {faturamentoTotal:F2}");
            Console.WriteLine("=========================");
            
            Console.ReadKey();
        }
    }

    public class OpcaoEditarPerfilCliente : IOpcaoMenu
    {
        private Usuario _usuarioLogado;
        public string Titulo => "Editar Meu Perfil";

        public OpcaoEditarPerfilCliente(Usuario usuario)
        {
            _usuarioLogado = usuario;
        }

        public void Executar()
        {
            Console.Clear();
            Console.WriteLine("=== EDITAR MEU PERFIL ===");
            
            Console.Write($"Novo Nome (Atual: {_usuarioLogado.Nome}): ");
            string nome = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nome)) _usuarioLogado.Nome = nome;

            Console.Write($"Novo CPF (Atual: {_usuarioLogado.Cpf}): ");
            string cpf = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(cpf)) _usuarioLogado.Cpf = cpf;

            Console.Write($"Novo Email (Atual: {_usuarioLogado.Email}): ");
            string email = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(email)) _usuarioLogado.Email = email;

            Console.Write($"Nova Senha (Atual: {_usuarioLogado.Senha}): ");
            string senha = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(senha)) _usuarioLogado.Senha = senha;

            _usuarioLogado.EditarPerfil();
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