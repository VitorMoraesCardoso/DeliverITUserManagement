using UserManagement.Domain.Entities;

namespace UserManagement.Tests.Domain;

public class UserTests
{
    #region CreateUser
    [Fact]
    public void CreateUser_Correctly()
    {
        var nome = "Monkey D. Luffy";
        var email = "luffy@gmail.com";
        var dataNascimento = DateTime.UtcNow.AddYears(-25);
        
        var user = new User(nome, email, dataNascimento);
        
        Assert.NotNull(user);
        Assert.Equal(nome, user.Nome);
        Assert.Equal(email, user.Email);
        Assert.Equal(dataNascimento.Date, user.DataNascimento.Date);
        Assert.True(user.DataCriacao <= DateTime.UtcNow);
        Assert.True(user.DataEdicao <= DateTime.UtcNow);
    }
    
    [Fact]
    public void CreateUser_FutureDate_Should_Throw_Argument_Exception()
    {
        var nome = "Bonney";
        var email = "bonney@gmail.com";
        var dataNascimento = DateTime.UtcNow.AddYears(25);
        
        var exception = Assert.Throws<ArgumentException>(() => new User(nome, email, dataNascimento));
        
        Assert.Contains("A data de nascimento não pode ser futura", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void CreateUser_InvalidName_Should_Throw_Argument_Exception(string nomeInvalido)
    {
        var email = "nomeInvalido@gmail.com";
        var dataNascimento = DateTime.UtcNow.AddYears(-25);
        
        Assert.Throws<ArgumentException>(() => new User(nomeInvalido, email, dataNascimento));
    }

    [Fact]
    public void CreateUser_NameBiggerThan50_Should_Throw_Argument_Exception()
    {
        var nome = new string('a', 51);
        var email = "nomeGigante@gmail.com";
        var dataNascimento = DateTime.UtcNow.AddYears(-25);
        
        Assert.Throws<ArgumentException>(() => new User(nome, email, dataNascimento));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void CreateUser_InvalidEmail_Should_Throw_Argument_Exception(string emailInvalido)
    {
        var nome = "Monkey D. Luffy";
        var dataNascimento = DateTime.UtcNow.AddYears(-25);
        
        Assert.Throws<ArgumentException>(() => new User(nome, emailInvalido, dataNascimento));
    }

    [Fact]
    public void CreateUser_EmailBiggerThan254_Should_Throw_Argument_Exception()
    {
        var nome = "Monkey D. Luffy";
        var email = new string('a', 255) + "@gmail.com";
        var dataNascimento = DateTime.UtcNow.AddYears(-25);
        
        Assert.Throws<ArgumentException>(() => new User(nome, email, dataNascimento));
    }
    
    #endregion
    
    #region UpdateUser

    [Fact]
    public void UpdateUser_Correctly()
    {
        var userOriginal = new User(
            "Monkey D. Luffy",
            "luffy@gmail.com",
            DateTime.UtcNow.AddYears(-30)
            );
        
        var nomeNovo = "Monkey D. Luffy (Atualizado)";
        var emailNovo = "luffyAtualizado@gmail.com";
        var dataNascimentoNova = DateTime.UtcNow.AddYears(-25);
        var dataCriacaoOriginal = userOriginal.DataCriacao;
        
        userOriginal.Update(nomeNovo, emailNovo, dataNascimentoNova);
        
        Assert.Equal(nomeNovo, userOriginal.Nome);
        Assert.Equal(emailNovo.ToLower(), userOriginal.Email);
        Assert.Equal(dataNascimentoNova.Date, userOriginal.DataNascimento.Date);
        Assert.Equal(dataCriacaoOriginal.Date, userOriginal.DataCriacao.Date);
        Assert.True(userOriginal.DataEdicao.Date >= userOriginal.DataCriacao.Date);
    }
    #endregion
}