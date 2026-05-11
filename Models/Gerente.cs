namespace novoprojeto;

public class Gerente : Usuario
{
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

    _todasAsQuadras.Add(quadra);
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

  /// <summary>
  /// Libera uma quadra após o aluguel
  /// </summary>
  public void LiberarAluguel(Quadra quadra)
  {
    if (quadra == null)
      throw new ArgumentNullException(nameof(quadra), "Quadra não pode ser nula");

    if (_quadrasAlugadas.Remove(quadra))
    {
      quadra.Disponivel = true;
    }
  }

  /// <summary>
  /// Obtém todas as quadras alugadas em uma data específica
  /// </summary>
  public List<Quadra> ObterAluguelsPorData(DateTime data)
  {
    return _quadrasAlugadas
        .Where(q => q.DataAluguel.Date == data.Date)
        .ToList();
  }

  /// <summary>
  /// Obtém todas as quadras alugadas por um usuário específico
  /// </summary>
  public List<Quadra> ObterAluguelsPorUsuario(Usuario usuario)
  {
    return _quadrasAlugadas
        .Where(q => q.UsuarioAluguel.Email == usuario.Email)
        .ToList();
  }

  /// <summary>
  /// Obtém todas as quadras alugadas em um horário específico
  /// </summary>
  public List<Quadra> ObterAluguelsPorHorario(TimeSpan horaInicio, TimeSpan horaFim)
  {
    return _quadrasAlugadas
        .Where(q => q.HoraInicio == horaInicio && q.HoraFim == horaFim)
        .ToList();
  }

  /// <summary>
  /// Gera um relatório de todas as quadras alugadas
  /// </summary>
  public string GerarRelatórioAluguel()
  {
    if (_quadrasAlugadas.Count == 0)
      return "Nenhuma quadra alugada no momento.";

    var relatorio = new System.Text.StringBuilder();
    relatorio.AppendLine("===== RELATÓRIO DE ALUGUEL DE QUADRAS =====");
    relatorio.AppendLine($"Total de quadras alugadas: {_quadrasAlugadas.Count}");
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

  /// <summary>
  /// Verifica se uma quadra está disponível em um horário específico
  /// </summary>
  public bool VerificarDisponibilidade(string nomeQuadra, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
  {
    var quadraEmUso = _quadrasAlugadas.FirstOrDefault(q =>
        q.Nome == nomeQuadra &&
        q.DataAluguel.Date == data.Date &&
        !(horaFim <= q.HoraInicio || horaInicio >= q.HoraFim));

    return quadraEmUso == null;
  }
}
