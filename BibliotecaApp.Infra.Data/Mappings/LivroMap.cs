using BibliotecaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Infra.Data.Mappings
{
    public class LivroMap : IEntityTypeConfiguration<Livro>
    {
        public void Configure(EntityTypeBuilder<Livro> builder)
        {
            builder.HasKey(l => l.Codl);

            builder.Property(l => l.Codl)
                .ValueGeneratedOnAdd();

            builder.HasIndex(l => l.Codl).IsUnique(); // Índice na pk

            builder.Property(l => l.Titulo)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(l => l.Editora)
                .HasMaxLength(40);

            builder.Property(l => l.Edicao);

            builder.Property(l => l.AnoPublicacao)
                .HasMaxLength(4);

            #region Relacionamentos
            builder.HasMany(l => l.Autores) // muitos-para-muitos com Autor através de LivroAutor
                .WithOne(la => la.Livro)
                .HasForeignKey(la => la.LivroCodl)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Assuntos) //muitos-para-muitos com Assunto através de LivroAssunto
                .WithOne(la => la.Livro)
                .HasForeignKey(la => la.LivroCodl)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
