namespace novoprojeto;

public class Categorias
{
    public string Nome { get; set; }
    public string Descricao { get; set; }

    public Categorias(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }
}