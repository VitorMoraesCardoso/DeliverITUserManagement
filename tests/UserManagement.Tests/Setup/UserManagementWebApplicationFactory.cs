using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.API.Endpoints;
using UserManagement.Infra.Context;

namespace UserManagement.Tests.Setup;

public class UserManagementWebApplicationFactory : WebApplicationFactory<IEndpoint>
{
    private SqliteConnection? _connection;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {       
        builder.ConfigureServices(services =>
        {
            //Removendo o postgres e testando com outro banco ajuda a demonstrar que a aplicação é flexível e
            //pode ser adaptada para diferentes bancos de dados, o que é uma boa prática de desenvolvimento.
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext) ||
                d.ServiceType.Name.Contains("DbContextOptionsConfiguration") ||
                d.ServiceType.Name.Contains("ConfigureDbContextOptions")).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
            
            //Criando um service provider interno e isolado pro SQLite pra nao conflitar com os servicos do
            //postgres que ja foram registrados globalmente, garantindo que os testes usem o banco em memória
            //sem interferir na configuração global da aplicação.
            var sqlInternalServiceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();
            
            //Criando uma conexão com um banco de dados em memória usando SQLite para os testes,
            //garantindo que cada teste tenha um ambiente limpo e isolado.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
                options.UseInternalServiceProvider(sqlInternalServiceProvider);
            });
            
            //Criando o banco e aplicando as migrations para garantir a estrutura do banco correta para os testes.
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Close();
        _connection?.Dispose();
    }
}