namespace novoprojeto;

public class Quadra
{
  public int Id { get; set; }
  public string Nome { get; set; }
  public Categorias Categoria { get; set; }
  public Usuario UsuarioAluguel { get; set; }
  public DateTime DataAluguel { get; set; }
  public TimeSpan HoraInicio { get; set; }
  public TimeSpan HoraFim { get; set; }
  public decimal Valor { get; set; }
  public bool Disponivel { get; set; }

  public Quadra(int id, string nome, Categorias categoria, Usuario usuarioAluguel,
                DateTime dataAluguel, TimeSpan horaInicio, TimeSpan horaFim, decimal valor)
  {
    Id = id;
    Nome = nome;
    Categoria = categoria;
    UsuarioAluguel = usuarioAluguel;
    DataAluguel = dataAluguel;
    HoraInicio = horaInicio;
    HoraFim = horaFim;
    Valor = valor;
    Disponivel = true;
  }

  public string ObterDetalhes()
  {
    return $"Quadra: {Nome} | Categoria: {Categoria.Nome} | " +
           $"Data: {DataAluguel:dd/MM/yyyy} | Horário: {HoraInicio:hh\\:mm} às {HoraFim:hh\\:mm} | " +
           $"Usuário: {UsuarioAluguel.Name} | Valor: R$ {Valor:F2}";
  }
}
