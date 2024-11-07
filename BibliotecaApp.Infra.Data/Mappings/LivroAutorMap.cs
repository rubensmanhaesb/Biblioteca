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
    public class LivroAutorMap : IEntityTypeConfiguration<LivroAutor>
    {
        public void Configure(EntityTypeBuilder<LivroAutor> builder)
        {
            builder.HasKey(la => new { la.LivroCodl, la.AutorCodAu });

            builder.HasIndex(la => la.LivroCodl); // Índice na FK 
            builder.HasIndex(la => la.AutorCodAu); // Índice na FK

            builder.HasOne(la => la.Livro)
                .WithMany(l => l.Autores)
                .HasForeignKey(la => la.LivroCodl)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(la => la.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(la => la.AutorCodAu)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
