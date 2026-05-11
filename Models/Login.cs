namespace novoprojeto;

public class Login
{
  public Login(string email, string senha)
  {
    Email = email;
    Senha = senha;
  }
  private string _email = string.Empty;
  private string _senha = string.Empty;
  public string Email
  {
    get => _email.ToUpper();
    set => _email = value;
  }
  public string Senha
  {
    get => _senha;
    set => _senha = value;
  }
}