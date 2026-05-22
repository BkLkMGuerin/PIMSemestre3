using System.Security.Cryptography;
using System.Text;

namespace novoprojeto;

public class Login
{
  private string _email = string.Empty;
  private string _passwordHash = string.Empty; // formato: salt:hash (Base64)

  public Login(string email)
  {
    Email = email;
  }

  public string Email
  {
    get => _email.ToUpper();
    private set => _email = value;
  }

  public void DefinirSenha(string senha)
  {
    var salt = RandomNumberGenerator.GetBytes(16);
    var combinado = Encoding.UTF8.GetBytes(Convert.ToBase64String(salt) + senha);
    var hash = SHA256.HashData(combinado);
    _passwordHash = $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
  }

  public bool VerificarSenha(string senha)
  {
    if (string.IsNullOrEmpty(_passwordHash))
      return false;

    try
    {
      var parts = _passwordHash.Split(':');
      if (parts.Length != 2)
        return false;

      var salt = Convert.FromBase64String(parts[0]);
      var hashEsperado = Convert.FromBase64String(parts[1]);

      var combinado = Encoding.UTF8.GetBytes(Convert.ToBase64String(salt) + senha);
      var hashAtual = SHA256.HashData(combinado);

      return CryptographicOperations.FixedTimeEquals(hashAtual, hashEsperado);
    }
    catch (FormatException)
    {
      return false;
    }
    catch (CryptographicException)
    {
      return false;
    }
    catch
    {
      return false;
    }
  }
}