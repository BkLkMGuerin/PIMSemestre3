using System;
using System.Collections.Generic;
using System.Linq;

namespace novoprojeto;

public class SistemaGerenciamento
{
  private readonly List<Usuario> _usuarios = new List<Usuario>();
  private readonly List<Gerente> _gerentes = new List<Gerente>();
  private readonly List<Quadra> _quadras = new List<Quadra>();

  public List<Usuario> Usuarios => _usuarios;
  public List<Gerente> Gerentes => _gerentes;
  public List<Quadra> Quadras => _quadras;

  public SistemaGerenciamento()
  {
    var administrador = new Gerente("ADMIN", "11999999999", "admin@admin.com");
    var loginAdministrador = new Login("admin@admin.com");
    loginAdministrador.DefinirSenha("admin");
    administrador.DefinirLogin(loginAdministrador);

    _usuarios.Add(administrador);
    _gerentes.Add(administrador);
  }

  public (bool sucesso, string mensagem) RegistrarUsuario(string nome, string telefone, string email, string senha)
  {
    if (EmailExiste(email))
      return (false, "Email já cadastrado.");

    if (string.IsNullOrWhiteSpace(senha))
      return (false, "Senha não pode ser vazia.");

    try
    {
      var usuario = new Usuario(nome, telefone, email);
      var login = new Login(email);
      login.DefinirSenha(senha);
      usuario.DefinirLogin(login);
      _usuarios.Add(usuario);
      return (true, "Usuário registrado.");
    }
    catch (ArgumentException ex)
    {
      return (false, $"Falha no registro: {ex.Message}");
    }
    catch (Exception ex)
    {
      return (false, $"Erro inesperado no registro: {ex.Message}");
    }
  }

  public (bool sucesso, string mensagem) RegistrarGerente(string nome, string telefone, string email, string senha)
  {
    if (EmailExiste(email))
      return (false, "Email já cadastrado.");

    if (string.IsNullOrWhiteSpace(senha))
      return (false, "Senha não pode ser vazia.");

    try
    {
      var gerente = new Gerente(nome, telefone, email);
      var login = new Login(email);
      login.DefinirSenha(senha);
      gerente.DefinirLogin(login);
      _usuarios.Add(gerente);
      _gerentes.Add(gerente);
      return (true, "Gerente registrado.");
    }
    catch (ArgumentException ex)
    {
      return (false, $"Falha no registro: {ex.Message}");
    }
    catch (Exception ex)
    {
      return (false, $"Erro inesperado no registro: {ex.Message}");
    }
  }

  public Usuario? Autenticar(string email, string senha)
  {
    var usuario = _usuarios.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    if (usuario == null)
      return null;

    return usuario.VerificarSenha(senha) ? usuario : null;
  }

  public bool EmailExiste(string email)
  {
    return _usuarios.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
  }
}
