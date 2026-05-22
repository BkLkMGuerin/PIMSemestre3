using System.Collections.Generic;
using System.Net.Mail;

namespace novoprojeto;

public class Usuario : IMenu
{
    private Login? _login;

    public Usuario(string name, string telefone, string email)
    {
        Name = name;
        Telefone = telefone;
        Email = email;
    }

    private string _name = string.Empty;
    private string _telefone = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name.ToUpper();
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Nome não deve ser vazio");
            }
            _name = value;
        }
    }
    public string Telefone
    {
        get => _telefone;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Telefone não deve ser vazio");
            }

            // Remove caracteres não numéricos para validação
            string apenasNumeros = new string(value.Where(char.IsDigit).ToArray());

            if (apenasNumeros.Length != 11)
            {
                throw new ArgumentException("Telefone deve conter exatamente 11 números");
            }

            _telefone = value;
        }
    }
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email não deve ser vazio");
            }

            try
            {
                // Valida se o email possui estrutura válida
                new MailAddress(value);
            }
            catch
            {
                throw new ArgumentException("Email deve possuir um formato válido (exemplo: usuario@dominio.com)");
            }

            _email = value;
        }
    }

    public void DefinirLogin(Login login)
    {
        _login = login;
    }

    public bool PossuiLogin() => _login != null;

    public bool VerificarSenha(string senha)
    {
        if (_login == null)
            return false;

        return _login.VerificarSenha(senha);
    }

    public virtual void MostrarMenu(List<Usuario> usuarios, List<Quadra> quadras, List<Gerente> gerentes)
    {
        while (true)
        {
            Console.WriteLine("\n--- Menu Usuário ---");
            Console.WriteLine("1 - Ver quadras disponíveis");
            Console.WriteLine("2 - Reservar quadra");
            Console.WriteLine("0 - Sair (logout)");
            Console.Write("Escolha: ");
            var op = Console.ReadLine();
            if (op == "0") break;

            if (op == "1")
            {
                if (!File.Exists("alugueis.txt"))
                {
                    Console.WriteLine("Nenhuma Quadra foi Encontrada ou (Arquivo nao encontrado)");
                    return;
                }
                else
                {
                    Console.WriteLine("====== QUADRAS CADASTRADAS ======");
                    foreach (string linha in File.ReadLines("alugueis.txt"))
                    {
                        if (!string.IsNullOrWhiteSpace(linha))
                        {
                            try
                            {
                                string[] partes = linha.Split('|');

                                string idPart = partes[0].Replace("Id:", "").Trim();
                                string nomePart = partes[1].Replace("Nome:", "").Trim();
                                string valorPart = partes[2].Replace("Valor: R$", "").Trim();

                                Console.WriteLine($"[Nº {idPart}] Quadra: {nomePart} - Preço: R$ {valorPart}");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"Linha formatada de forma diferente: {linha}");
                            }
                        }
                        Console.WriteLine("=================================");
                    }
                    {

                    }
                }
            }
            else if (op == "2")
            {
                Console.Write("Informe o N° da quadra: ");
                if (!int.TryParse(Console.ReadLine(), out var id))
                {
                    Console.WriteLine("N° inválido.");
                    continue;
                }

                var quadra = quadras.FirstOrDefault(q => q.Id == id);
                if (quadra == null )
                {
                    Console.WriteLine("Quadra não disponível.");
                    continue;
                }
                if (!quadra.Disponivel)
                {
                    Console.WriteLine($"A quadra '{quadra.Nome}' já está ocupada ou indisponível no momento.");
                    continue;
                }

                Console.Write("Data (dd/MM/yyyy): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var data))
                {
                    Console.WriteLine("Data inválida.");
                    continue;
                }

                if (data.Date < DateTime.Today)
                {
                    Console.WriteLine("Não é possível reservar para data anterior a hoje.");
                    continue;
                }

                Console.Write("Hora início (HH:mm): ");
                if (!TimeSpan.TryParse(Console.ReadLine(), out var hi))
                {
                    Console.WriteLine("Horário inválido.");
                    continue;
                }

                Console.Write("Hora fim (HH:mm): ");
                if (!TimeSpan.TryParse(Console.ReadLine(), out var hf))
                {
                    Console.WriteLine("Horário inválido.");
                    continue;
                }

                // atribui aluguel
                quadra.DataAluguel = data;
                quadra.UsuarioAluguel = this;
                quadra.HoraInicio = hi;
                quadra.HoraFim = hf;
                System.Console.WriteLine($"{quadra}");
                var disponivelAntigo = quadra.Disponivel;
                try
                {
                    var gerente = gerentes.FirstOrDefault();
                    if (gerente == null)
                    {
                        throw new InvalidOperationException("Nenhum gerente disponível para registrar a reserva.");
                    }

                    gerente.RegistrarAluguel(quadra);
                    quadra.Disponivel = false;
                    Console.WriteLine("Reserva efetuada com sucesso.");
                }
                catch (InvalidOperationException ex)
                {
                    quadra.Disponivel = disponivelAntigo;
                    Console.WriteLine($"Não foi possível reservar: {ex.Message}");
                }
                catch (Exception ex)
                {
                    quadra.Disponivel = disponivelAntigo;
                    Console.WriteLine($"Erro ao efetuar reserva: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Opção inválida.");
            }
        }
    }
}
