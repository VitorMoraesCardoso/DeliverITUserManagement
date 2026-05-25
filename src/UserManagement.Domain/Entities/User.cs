namespace UserManagement.Domain.Entities;

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
        ValidateDomainInvariants(nome, email, dataNascimento);
        
        Nome = nome;
        Email = email.Trim().ToLower();
        DataNascimento = dataNascimento.Date;
        DataCriacao = DateTime.UtcNow;
        DataEdicao = DateTime.UtcNow;
    }
    
    public void Update(string nome, string email, DateTime dataNascimento)
    {
        ValidateDomainInvariants(nome, email, dataNascimento);
        
        Nome = nome;
        Email = email.Trim().ToLower();
        DataNascimento = dataNascimento.Date;
        DataEdicao = DateTime.UtcNow;
    }

    //Validacoes que bloqueam o time de cadastrar dados invalidos via codigo
    private void ValidateDomainInvariants(string nome, string email, DateTime dataNascimento)
    {
        if (string.IsNullOrEmpty(nome))
        {
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));
        }
        
        if (nome.Trim().Length == 0)
        {
            throw new ArgumentException("O nome não pode ser vazio ou conter apenas espaços em branco.", nameof(nome));
        }

        if (nome.Length > 50)
        {
            throw new ArgumentException("O nome deve conter no máximo 50 caracteres.", nameof(nome));
        }

        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("O email é obrigatório.", nameof(email));
        }

        if (email.Trim().Length == 0)
        {
            throw new ArgumentException("O email não pode ser vazio ou conter apenas espaços em branco.", nameof(email));
        }

        if (email.Length > 254)
        {
            throw new ArgumentException("O email deve conter no máximo 254 caracteres.", nameof(email));
        }

        if (dataNascimento.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("A data de nascimento não pode ser futura.", nameof(dataNascimento));
        }
    }
}

