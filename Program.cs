//TODO: ERRO AO MOSTRAR QUADRAS PARA O GERENTE
// TODO: ERRO NA HORA DE CADASTRAR QUADRA
// TODO: REGISTRAR EM TXT OS USUARIOS CADASTRADOS COM HASH NA SENHA
using novoprojeto;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

class Program
{
  static void Main()
  {
    var sistema = new SistemaGerenciamento();

    Console.WriteLine("Sistema de Gerenciamento de Quadras - protótipo (terminal)");

    while (true)
    {
      Console.WriteLine("\n--- Tela Inicial ---");
      Console.WriteLine("1 - Registrar");
      Console.WriteLine("2 - Login");
      Console.WriteLine("3 - Teste Rápido (Usuário)");
      Console.WriteLine("4 - Teste Rápido (Gerente)");
      Console.WriteLine("0 - Sair");
      Console.Write("Escolha: ");
      var escolha = Console.ReadLine();
      if (escolha == "0") break;

      // Testes rápidos
      if (escolha == "3")
      {
        var usuarioTeste = new Usuario("Usuário Teste", "12345678901", "teste@usuario.com");
        var loginTeste = new Login("teste@usuario.com");
        loginTeste.DefinirSenha("1234");
        usuarioTeste.DefinirLogin(loginTeste);
        if (!sistema.Usuarios.Any(u => u.Email == "teste@usuario.com"))
        {
          sistema.Usuarios.Add(usuarioTeste);
        }
        Console.WriteLine(" Logado como Usuário Teste");
        usuarioTeste.MostrarMenu(sistema.Usuarios, sistema.Quadras, sistema.Gerentes);
      }
      else if (escolha == "4")
      {
        var gerenteTeste = new Gerente("Gerente Teste", "98765432101", "teste@gerente.com");
        var loginTeste = new Login("teste@gerente.com");
        loginTeste.DefinirSenha("1234");
        gerenteTeste.DefinirLogin(loginTeste);
        if (!sistema.Gerentes.Any(g => g.Email == "teste@gerente.com"))
        {
          sistema.Usuarios.Add(gerenteTeste);
          sistema.Gerentes.Add(gerenteTeste);
        }
        Console.WriteLine(" Logado como Gerente Teste");
        gerenteTeste.MostrarMenu(sistema.Usuarios, sistema.Quadras, sistema.Gerentes);
      }
      else if (escolha == "1")
      {
        Console.Write("Registrar como (1-Gerente, 2-Usuário): ");
        var tipo = Console.ReadLine();

        if (tipo != "1" && tipo != "2")
        {
          Console.WriteLine("Opção inválida para registro. Tente novamente.");
          continue;
        }

        while (true)
        {
          Console.Write("Nome: "); var nome = Console.ReadLine() ?? string.Empty;
          Console.Write("Telefone: "); var tel = Console.ReadLine() ?? string.Empty;
          Console.Write("Email: "); var email = Console.ReadLine() ?? string.Empty;
          Console.Write("Senha: "); var senha = Console.ReadLine() ?? string.Empty;

          if (!ValidarDadosRegistro(nome, tel, email, senha, out var mensagemErro))
          {
            Console.WriteLine(mensagemErro);
            Console.Write("Dados incorretos. Deseja voltar ao menu principal? (S/N): ");
            var voltarMenu = Console.ReadLine();
            if (string.Equals(voltarMenu, "S", StringComparison.OrdinalIgnoreCase))
              break;
            Console.WriteLine("Por favor, insira os dados novamente.");
            continue;
          }

          var resultado = tipo == "1"
            ? sistema.RegistrarGerente(nome, tel, email, senha)
            : sistema.RegistrarUsuario(nome, tel, email, senha);

          Console.WriteLine(resultado.mensagem);

          if (resultado.sucesso)
            break;

          Console.Write("Deseja tentar novamente? (S/N): ");
          var tentarNovamente = Console.ReadLine();
          if (string.Equals(tentarNovamente, "S", StringComparison.OrdinalIgnoreCase))
            continue;

          Console.Write("Deseja voltar ao menu principal? (S/N): ");
          var voltarAoMenu = Console.ReadLine();
          if (string.Equals(voltarAoMenu, "S", StringComparison.OrdinalIgnoreCase))
            break;
          Console.WriteLine("Continuando o registro. Insira os dados novamente.");
        }
      }
      else if (escolha == "2")
      {
        Console.Write("Email: "); var email = Console.ReadLine() ?? string.Empty;
        Console.Write("Senha: "); var senha = Console.ReadLine() ?? string.Empty;

        var usuario = sistema.Autenticar(email, senha);
        if (usuario == null)
        {
          Console.WriteLine("Credenciais inválidas.");
          continue;
        }

        Console.WriteLine($"Bem-vindo(a), {usuario.Name}!");

        // Se for gerente, mostra o menu do gerente normalmente
        if (usuario is Gerente)
        {
          usuario.MostrarMenu(sistema.Usuarios, sistema.Quadras, sistema.Gerentes);
        }
        else
        {
          // Para usuário comum, se não houver quadras disponíveis mostra mensagem e não exibe opções
          var existemDisponiveis = sistema.Quadras.Any(q => q.Disponivel);
          if (!existemDisponiveis)
          {
            Console.WriteLine("nenhuma quadra disponivel no momento");
          }
          else
          {
            usuario.MostrarMenu(sistema.Usuarios, sistema.Quadras, sistema.Gerentes);
          }
        }
      }
      else
      {
        Console.WriteLine("Opção inválida.");
      }
    }

    Console.WriteLine("Encerrando...");
  }

  static bool ValidarDadosRegistro(string nome, string telefone, string email, string senha, out string mensagemErro)
  {
    if (string.IsNullOrWhiteSpace(nome))
    {
      mensagemErro = "Nome não deve ser vazio.";
      return false;
    }

    if (string.IsNullOrWhiteSpace(telefone))
    {
      mensagemErro = "Telefone não deve ser vazio.";
      return false;
    }

    var apenasNumeros = new string(telefone.Where(char.IsDigit).ToArray());
    if (apenasNumeros.Length != 11)
    {
      mensagemErro = "Telefone deve conter exatamente 11 números.";
      return false;
    }

    if (string.IsNullOrWhiteSpace(email))
    {
      mensagemErro = "Email não deve ser vazio.";
      return false;
    }

    try
    {
      new MailAddress(email);
    }
    catch
    {
      mensagemErro = "Email deve possuir um formato válido (exemplo: usuario@dominio.com).";
      return false;
    }

    if (string.IsNullOrWhiteSpace(senha))
    {
      mensagemErro = "Senha não deve ser vazia.";
      return false;
    }

    if (senha.Length < 4)
    {
      mensagemErro = "Senha deve ter ao menos 4 caracteres.";
      return false;
    }

    mensagemErro = string.Empty;
    return true;
  }
}
