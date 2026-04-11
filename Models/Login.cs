namespace novoprojeto;

public class Login
{
  public Login(string email, string senha)
  {
    Email = email;
    Senha = senha;
  }
  private _email = string.Empty;
  private _senha = string.Empty;
  public string Email
  {
    get => _email.ToUpper();
  }
}