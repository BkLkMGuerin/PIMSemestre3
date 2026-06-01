namespace AluguelQuadrasSUN7;  

public class MenuPronto
{
    private readonly string _titulo;
    private readonly List<IOpcaoMenu> _opcoes;

    // O construtor obriga o menu a receber um título e a lista de opções dele
    public MenuPronto(string titulo, List<IOpcaoMenu> opcoes)
    {
        _titulo = titulo;
        _opcoes = opcoes;
    }

    public void Exibir()
    {
        bool executando = true;
        while (executando)
        {
            Console.Clear();
            Console.WriteLine($"=== {_titulo.ToUpper()} ===");

            // Desenha as opções na tela de 1 até o total de itens da lista
            for (int i = 0; i < _opcoes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_opcoes[i].Titulo}");
            }
            Console.WriteLine("0. Sair / Voltar");
            Console.WriteLine("==================");
            Console.Write("Escolha uma opção: ");

            // Garante que o usuário digitou um número inteiro válido
            if (int.TryParse(Console.ReadLine(), out int escolha))
            {
                if (escolha == 0)
                {
                    executando = false;
                }
                // Valida se o número digitado está dentro do intervalo de opções da lista
                else if (escolha > 0 && escolha <= _opcoes.Count)
                {
                    // POLIMORFISMO: Executa a classe correspondente ao número, sem switch case
                    _opcoes[escolha - 1].Executar();
                }
                else
                {
                    Console.WriteLine("\nOpção inexistente! Pressione qualquer tecla para tentar novamente...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("\nPor favor, digite um número válido! Pressione qualquer tecla...");
                Console.ReadKey();
            }
        }
    }
}