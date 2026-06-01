namespace AluguelQuadrasSUN7
;

public enum TipoUsuario
{
    Cliente,
    Gestor
}

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    private string pathFile = "usuarios.txt";
    public TipoUsuario Tipo { get; set; }

        public void Cadastrar()
    {
        try
        {
            if (File.Exists(pathFile))
            {
                string[] linhas = File.ReadAllLines(pathFile);
                this.Id = linhas.Length + 1;
            }
            else
            {
                this.Id = 1;
            }
            string dadosUsuario = $"{Id}|{Nome}|{Cpf}|{Email}|{Senha}|{Tipo}|{DateTime.Now}\n";
            
            File.AppendAllText(pathFile, dadosUsuario);
            Console.WriteLine("Usuario cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao cadastrar usuario: " + ex.Message);
        }
    }

    public bool Login(string emailDigitado, string senhaDigitada)
{
    if (!File.Exists(pathFile))
    {
        Console.WriteLine("Nenhum usuário cadastrado!");
        return false;
    }

    try
    {
        string[] linhas = File.ReadAllLines(pathFile);
        foreach (var linha in linhas)
        {
            if (string.IsNullOrWhiteSpace(linha)) continue;

            string[] dados = linha.Split("|");
            if (dados.Length < 6) continue;

            string emailDoArquivo = dados[3];
            string senhaDoArquivo = dados[4]; // Este agora é o HASH do BCrypt

            // 1. Compara o e-mail normalmente
            // 2. Usa o método Security.VerificarSenha para validar a senha digitada contra o hash do arquivo
            if (emailDoArquivo.Equals(emailDigitado, StringComparison.OrdinalIgnoreCase) && 
                Security.VerificarSenha(senhaDigitada, senhaDoArquivo))
            {
                this.Id = int.Parse(dados[0]);
                this.Nome = dados[1];
                this.Cpf = dados[2];
                this.Email = emailDoArquivo;
                this.Senha = senhaDoArquivo; // Guarda o hash ou limpa por segurança
                this.Tipo = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), dados[5]);

                Console.WriteLine($"\nLogin realizado com sucesso! Bem-vindo(a), {this.Nome}.");
                return true;
            }
        }
        
        // Se percorreu todo o arquivo e não retornou true
        Console.WriteLine("\nE-mail ou senha incorretos.");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao tentar realizar o login: {ex.Message}");
        return false;
    }
}

          public void EditarPerfil()
    {

            if (this.Id == 0)
            {
                Console.WriteLine("Erro: Você precisa estar logado para editar o perfil.");
                return;
            }

            if (!File.Exists(pathFile))
            {
                Console.WriteLine("Erro: Arquivo de registros não encontrado.");
                return;
            }

            try
            {
            
                string[] linhas = File.ReadAllLines(pathFile);
                bool usuarioEncontrado = false;

                
                for (int i = 0; i < linhas.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(linhas[i])) continue;

                    string[] dados = linhas[i].Split('|');
                    int idDoArquivo = int.Parse(dados[0]);

                    if (idDoArquivo == this.Id)
                    {
                        
                        linhas[i] = $"{Id}|{Nome}|{Cpf}|{Email}|{Senha}|{Tipo}|{DateTime.Now}";
                        usuarioEncontrado = true;
                        break; 
                    }
                }

                if (usuarioEncontrado)
                {
                    //Reescreve o arquivo com as atualizações
                    File.WriteAllLines(pathFile, linhas);
                    Console.WriteLine("\nPerfil atualizado com sucesso no arquivo!");
                }
                else
                {
                    Console.WriteLine("Erro: Usuário não encontrado no arquivo para edição.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar o perfil: {ex.Message}");
            }
    }
    }