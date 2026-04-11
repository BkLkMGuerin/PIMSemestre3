namespace novoprojeto;

public class Usuario
{
    public Usuario(string name, string telefone, string email)
    {
        Name = name;
        Telefone = telefone;
        Email = email;
    }

    private string _name = string.Empty;
    private string _telefone = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name.ToUpper();
        set
        {
            if (value == "")
            {
                throw new ArgumentException("Nome não deve ser vazio");
            }
            _name = value;
        }
    }
    public string Telefone
    {
        get =>_telefone;
        set
        {
            if (value == "")
            {
                throw new ArgumentException("Telefone não deve ser vazio");
            }
        }
    }
    public string Email
    {
        get => _email;
        set
        {
            if (value == "")
            {
                throw new ArgumentException("Email não deve ser vazio");
            }
        }
    }
}
