using System;
using System.IO; // Adicionado para reconhecer a classe File

namespace novoprojeto;

public class Quadra
{
  public int Id { get; set; }
  public string Nome { get; set; }
  public Usuario UsuarioAluguel { get; set; }
  public DateTime DataAluguel { get; set; }
  public TimeSpan HoraInicio { get; set; }
  public TimeSpan HoraFim { get; set; }
  public decimal Valor { get; set; }
  public bool Disponivel { get; set; }
  private List<Quadra> _quadrasAlugadas = new List<Quadra>();
  private List<Quadra> _todasAsQuadras = new List<Quadra>();
  public IReadOnlyList<Quadra> QuadrasAlugadas => _quadrasAlugadas.AsReadOnly();
  public IReadOnlyList<Quadra> TodasAsQuadras => _todasAsQuadras.AsReadOnly();

  public Quadra(int id, string nome, Usuario usuarioAluguel,
                DateTime dataAluguel, TimeSpan horaInicio, TimeSpan horaFim, decimal valor)
  {
    Id = id;
    Nome = nome;
    UsuarioAluguel = usuarioAluguel;
    DataAluguel = dataAluguel;
    HoraInicio = horaInicio;
    HoraFim = horaFim;
    Valor = valor;
    Disponivel = true;
  }

  public Quadra(int id, string nome, decimal valor)
  {
    Id = id;
    Nome = nome;
    Valor = valor;
    Disponivel = true;

  }
  public void RegistrarAluguel(Quadra quadra)
  {
    if (quadra == null)
      throw new ArgumentNullException(nameof(quadra), "Quadra não pode ser nula");

    if (!_todasAsQuadras.Contains(quadra))
      throw new InvalidOperationException("Quadra não está cadastrada no sistema");

    _quadrasAlugadas.Add(quadra);
    quadra.Disponivel = false;
  }
  public string ObterDetalhes()
  {
    string nomeUsuario = UsuarioAluguel != null ? UsuarioAluguel.Name : "Ninguém";

    return $"Quadra: {Nome} | " +
           $"Data: {DataAluguel:dd/MM/yyyy} | Horário: {HoraInicio:hh\\:mm} às {HoraFim:hh\\:mm} | " +
           $"Usuário: {nomeUsuario} | Valor: R$ {Valor:F2}";
  }

  public Quadra ProcurarQuadraPorIdNoTxt(int idProcurado)
  {
    if (!File.Exists("alugueis.txt"))
    {
      Console.WriteLine("Arquivo de dados não encontrado.");
      return null;
    }
    string termoBusca = $"Id: {idProcurado} ";

    foreach (string linha in File.ReadLines("alugueis.txt"))
    {
      if (string.IsNullOrWhiteSpace(linha)) continue;

      if (linha.StartsWith(termoBusca))
      {
        try
        {
          string[] partes = linha.Split('|');

          int id = int.Parse(partes[0].Replace("Id:", "").Trim());
          string nome = partes[1].Replace("Nome:", "").Trim();

          decimal valor = decimal.Parse(partes[2].Replace("Valor: R$", "").Trim());

          Quadra quadraEncontrada = new Quadra(id, nome, valor);
          System.Console.WriteLine("PASSOU POR AQUI");
          return quadraEncontrada;
        }
        catch (Exception)
        {
          Console.WriteLine("Erro ao processar a linha do arquivo.");
          return null;
        }
      }
    }

    return null;
  }
}