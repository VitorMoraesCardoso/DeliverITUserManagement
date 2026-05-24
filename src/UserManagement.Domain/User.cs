namespace UserManagement.Domain;

public class User
{
    public int Id { get; private set; }
    
    public string Nome { get; private set; } =  string.Empty;
    public string Email { get; private set; } =  string.Empty;
    public DateTime DataNascimento { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataEdicao { get; private set; }
    
    private User() { }

    public User(string nome, string email, DateTime dataNascimento)
    {
        ValidateAndCreate(nome, email, dataNascimento);
        DataCriacao = DateTime.UtcNow;
        DataEdicao = DateTime.UtcNow;
    }
    
    public void Update(string nome, string email, DateTime dataNascimento)
    {
        ValidateAndCreate(nome, email, dataNascimento);
        DataEdicao = DateTime.UtcNow;
    }

    private void ValidateAndCreate(string nome, string email, DateTime dataNascimento)
    {
        
    }
}

