namespace novoprojeto;

public class Pagamento
{
  public Usuario Usuario { get; }
  public Login Login { get; }

  public Pagamento(Usuario usuario, Login login)
  {
    Usuario = usuario;
    Login = login;
  }

  public bool EstaLogado()
  {
    return Login != null
      && !string.IsNullOrWhiteSpace(Usuario.Email)
      && string.Equals(Usuario.Email, Login.Email, System.StringComparison.OrdinalIgnoreCase);
  }

  public string Efetuar(decimal valor)
  {
    if (!EstaLogado())
    {
      throw new InvalidOperationException("Usuário dvee estar logado para efetuar o pagamento.");
    }

    if (valor <= 0)
    {
      throw new ArgumentException("Valor do pagamento deve ser maior que zero.", nameof(valor));// esse nameof muda sozinho se a gente mudar o nome da variável valor
    }

    return $"Pagamento de R$ {valor:F2} realizado por: {Usuario.Name}.";
  }
}
