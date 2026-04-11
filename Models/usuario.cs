using System.Dynamic;

namespace novoprojeto;

public class usuario
{
    public usuario(string name, string telefone)
    {
        Name = name;
        Telefone = telefone;
    }

    private string _name = string.Empty;
    private string _telefone = string.Empty;

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
}
