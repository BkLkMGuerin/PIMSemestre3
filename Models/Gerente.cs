using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Data.Common;
namespace novoprojeto;

public class Gerente : Usuario
{
  public string pathfile = "alugueis.txt";
  public string novaQuadra = Environment.NewLine;
  private List<Quadra> _quadrasAlugadas = new List<Quadra>();
  private List<Quadra> _todasAsQuadras = new List<Quadra>();

  public IReadOnlyList<Quadra> QuadrasAlugadas => _quadrasAlugadas.AsReadOnly();
  public IReadOnlyList<Quadra> TodasAsQuadras => _todasAsQuadras.AsReadOnly();

  public Gerente(string name, string telefone, string email)
      : base(name, telefone, email)
  {
  }

  /// <summary>
  /// Adiciona uma quadra ao sistema de gerenciamento
  /// </summary>
  public void AdicionarQuadra(Quadra quadra)
  {
    if (quadra == null)
      throw new ArgumentNullException(nameof(quadra), "Quadra não pode ser nula");

    File.AppendAllText(pathfile, novaQuadra + $"Id: {quadra.Id} | Nome: {quadra.Nome} | Valor: R$ {quadra.Valor:F2}");
  }

  /// <summary>
  /// Registra um aluguel de quadra
  /// </summary>
  public void RegistrarAluguel(Quadra quadra)
  {
    if (quadra == null)
      throw new ArgumentNullException(nameof(quadra), "Quadra não pode ser nula");

    if (!_todasAsQuadras.Contains(quadra))
      throw new InvalidOperationException("Quadra não está cadastrada no sistema");

    _quadrasAlugadas.Add(quadra);
    quadra.Disponivel = false;
  }

  public void LiberarAluguel(Quadra quadra)
  {
    if (quadra == null)
      throw new ArgumentNullException(nameof(quadra), "Quadra não pode ser nula");

    if (_quadrasAlugadas.Remove(quadra))
    {
      quadra.Disponivel = true;
    }
  }

  public List<Quadra> ObterAluguelsPorData(DateTime data)
  {
    return _quadrasAlugadas
        .Where(q => q.DataAluguel.Date == data.Date)
        .ToList();
  }

  public List<Quadra> ObterAluguelsPorUsuario(Usuario usuario)
  {
    return _quadrasAlugadas
        .Where(q => q.UsuarioAluguel.Email == usuario.Email)
        .ToList();
  }
  public List<Quadra> ObterAluguelsPorHorario(TimeSpan horaInicio, TimeSpan horaFim)
  {
    return _quadrasAlugadas
        .Where(q => q.HoraInicio == horaInicio && q.HoraFim == horaFim)
        .ToList();
  }

  public string GerarRelatórioAluguel()
  {
    if (_quadrasAlugadas.Count == 0)
      return "Nenhuma quadra alugada no momento.";

    var relatorio = new System.Text.StringBuilder();
    relatorio.AppendLine("=x=x= RELATÓRIO DE ALUGUEL DE QUADRAS =x=x=");
    relatorio.AppendLine($"Número de quadras alugadas: {_quadrasAlugadas.Count}");
    relatorio.AppendLine(new string('-', 50));

    decimal totalArrecadado = 0;
    foreach (var quadra in _quadrasAlugadas)
    {
      relatorio.AppendLine(quadra.ObterDetalhes());
      totalArrecadado += quadra.Valor;
    }

    relatorio.AppendLine(new string('-', 50));
    relatorio.AppendLine($"Total arrecadado: R$ {totalArrecadado:F2}");

    return relatorio.ToString();
  }

  public override void MostrarMenu(List<Usuario> usuarios, List<Quadra> quadras, List<Gerente> gerentes)
  {
    while (true)
    {
      try
      {
        Console.WriteLine("\n--- Menu Gerente ---");
        Console.WriteLine("1 - Adicionar quadra");
        Console.WriteLine("2 - Listar todas as quadras");
        Console.WriteLine("3 - Liberar aluguel (por Id)");
        Console.WriteLine("4 - Ver relatório");
        Console.WriteLine("0 - Sair (logout)");
        Console.Write("Escolha: ");
        var op = Console.ReadLine();
        if (op == "0") break;

        if (op == "1")
        {
          try
          {

            Console.Write("Nome: ");
            var nome = Console.ReadLine() ?? string.Empty;
            Console.Write("Valor (ex: 120.50): ");
            if (!decimal.TryParse(Console.ReadLine(), out var valor)) { Console.WriteLine("Valor inválido."); continue; }

            var placeholderUser = new Usuario("SEM_USUARIO", "00000000000", "sem@null");
            var totalLinhas = File.ReadLines(pathfile).Count(linha => !string.IsNullOrWhiteSpace(linha));
            var id = totalLinhas + 1;
            var nova = new Quadra(id, nome, placeholderUser, DateTime.Now, TimeSpan.Zero, TimeSpan.Zero, valor);
            AdicionarQuadra(nova);
            quadras.Add(nova);
            Console.WriteLine("Quadra adicionada.");
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Falha ao adicionar quadra: {ex.Message}");
          }
        }
        else if (op == "2")
        {
          foreach (var q in quadras)
            Console.WriteLine($"Id: {q.Id} | Nome: {q.Nome} | Disponível: {q.Disponivel} | Valor: R$ {q.Valor:F2}");
          Console.WriteLine(quadras.Count == 0 ? "Nenhuma quadra cadastrada." : $"Total de quadras: {quadras.Count}");
        }

        else if (op == "3")
        {
          try
          {
            Console.Write("Id da quadra para liberar: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); continue; }
            var quadra = quadras.FirstOrDefault(q => q.Id == id);
            if (quadra == null)
            {
              Console.WriteLine("Quadra não encontrada.");
              continue;
            }
            LiberarAluguel(quadra);
            quadra.Disponivel = true;
            Console.WriteLine("Aluguel liberado (se estava alugado).");
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Falha ao liberar aluguel: {ex.Message}");
          }
        }
        else if (op == "4")
        {
          Console.WriteLine(GerarRelatórioAluguel());
        }
        else
        {
          Console.WriteLine("Opção inválida.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Erro no menu do gerente: {ex.Message}");
      }
    }
  }

  public bool VerificarDisponibilidade(string nomeQuadra, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
  {
    var quadraEmUso = _quadrasAlugadas.FirstOrDefault(q =>
        q.Nome == nomeQuadra &&
        q.DataAluguel.Date == data.Date &&
        !(horaFim <= q.HoraInicio || horaInicio >= q.HoraFim));

    return quadraEmUso == null;
  }
}
