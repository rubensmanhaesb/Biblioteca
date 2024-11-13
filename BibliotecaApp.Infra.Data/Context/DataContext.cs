using BibliotecaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaApp.Infra.Data.Mappings;
using BibliotecaApp.Infra.Data.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BibliotecaApp.Infra.Data.Context
{
    public class DataContext : DbContext
    {
        private readonly AsyncRetryPolicy retryPolicy;
        private readonly AsyncCircuitBreakerPolicy circuitBreakerPolicy;
        private readonly ILogger<DataContext> _logger;
        private bool _wasCircuitOpen;

        public CircuitState CircuitBreakerState => circuitBreakerPolicy.CircuitState;

        #region Constructors
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            
        }
        public DataContext(DbContextOptions<DataContext> options, ILogger<DataContext> logger) : base(options)
        {
            _logger = logger;

            // Configuração da política de Retry
            retryPolicy = Policy
                .Handle<SqlException>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogInformation($"Retry attempt {retryCount} at {DateTime.Now}: {exception.Message}. Waiting {timeSpan} before next retry.");
                    });

            // Configuração do Circuit Breaker
            circuitBreakerPolicy = Policy
                .Handle<SqlException>()
                .Or<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogWarning($"Circuit breaker opened at {DateTime.Now} due to: {exception.Message}. Breaking for {duration.TotalSeconds} seconds.");
                        _wasCircuitOpen = true;
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation($"Circuit breaker reset at {DateTime.Now}. Ready for new operations.");
                        _wasCircuitOpen = false;
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation($"Circuit breaker is half-open at {DateTime.Now}. Next call is a trial.");
                    });
        }
        #endregion Constructors

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Livro>()
                .Property(p => p.Codl)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Autor>()
                .Property(p => p.CodAu)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Assunto>()
                .Property(p => p.CodAs)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PrecoLivro>()
                .HasKey(p => p.Codp);

            modelBuilder.Entity<PrecoLivro>()
                .HasOne(p => p.Livro)
                .WithMany(l => l.Precos)
                .HasForeignKey(p => p.LivroCodl);

            modelBuilder.Entity<PrecoLivro>()
                .Property(p => p.TipoCompra)
                .HasConversion<int>();

            modelBuilder.Entity<LivroAutor>()
                .HasKey(la => new { la.LivroCodl, la.AutorCodAu });

            modelBuilder.Entity<LivroAutor>()
                .HasOne(la => la.Livro)
                .WithMany(l => l.Autores)
                .HasForeignKey(la => la.LivroCodl);

            modelBuilder.Entity<LivroAutor>()
                .HasOne(la => la.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(la => la.AutorCodAu);

            modelBuilder.Entity<LivroAssunto>()
                .HasKey(la => new { la.LivroCodl, la.AssuntoCodAs });

            modelBuilder.Entity<LivroAssunto>()
                .HasOne(la => la.Livro)
                .WithMany(l => l.Assuntos)
                .HasForeignKey(la => la.LivroCodl);

            modelBuilder.Entity<LivroAssunto>()
                .HasOne(la => la.Assunto)
                .WithMany(a => a.Livros)
                .HasForeignKey(la => la.AssuntoCodAs);

            modelBuilder.ApplyConfiguration(new AssuntoMap());
            modelBuilder.ApplyConfiguration(new AutorMap());
            modelBuilder.ApplyConfiguration(new LivroAssuntoMap());
            modelBuilder.ApplyConfiguration(new LivroAutorMap());
            modelBuilder.ApplyConfiguration(new LivroMap());
        }

        public static DbContextOptions<DataContext> CreateOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

            return optionsBuilder.Options;
        }

        public async Task<T> ExecuteWithPoliciesAsync<T>(Func<Polly.Context, Task<T>> action)
        {
            var policyContext = new Polly.Context { ["logger"] = _logger };

            try
            {
                // Executa o retry e o circuit breaker
                var result = await retryPolicy.ExecuteAsync(async ctx =>
                {
                    return await circuitBreakerPolicy.ExecuteAsync(action, ctx);
                }, policyContext);

                // Se o circuito estiver fechado após uma execução bem-sucedida
                if (circuitBreakerPolicy.CircuitState == CircuitState.Closed && _wasCircuitOpen)
                {
                    _logger.LogInformation($"Circuit breaker closed at {DateTime.Now:dd/MM/yyyy HH:mm:ss}. Service is now operational.");
                    _wasCircuitOpen = false;
                }

                return result;
            }
            catch (BrokenCircuitException ex)
            {
                _wasCircuitOpen = true;
                _logger.LogError("Circuit breaker open: " + ex.Message);
                throw new CircuitBreakerOpenException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Circuit breaker retry policy failed: " + ex.Message);
                throw new RetryException(ex);
            }
        }
    }
}
