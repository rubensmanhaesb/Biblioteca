using BibliotecaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaApp.Infra.Data.Mappings;

namespace BibliotecaApp.Infra.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<PrecoLivro>()
                .HasKey(p => p.Codp);

            modelBuilder.Entity<PrecoLivro>()
                .HasOne(p => p.Livro)
                .WithMany(l => l.Precos)
                .HasForeignKey(p => p.LivroCodl);

            // Mapeia o enum TipoCompra como int (opção padrão)
            modelBuilder.Entity<PrecoLivro>()
                .Property(p => p.TipoCompra)
                .HasConversion<int>();

            // Configuração da chave composta para LivroAutor
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

            // Configuração da chave composta para LivroAssunto
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

            // Aplicação das configurações adicionais
            modelBuilder.ApplyConfiguration(new AssuntoMap());
            modelBuilder.ApplyConfiguration(new AutorMap());
            modelBuilder.ApplyConfiguration(new LivroAssuntoMap());
            modelBuilder.ApplyConfiguration(new LivroAutorMap());
            modelBuilder.ApplyConfiguration(new LivroMap());
        }
    }
}
