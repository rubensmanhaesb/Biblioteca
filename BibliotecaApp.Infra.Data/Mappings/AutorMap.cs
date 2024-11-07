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
    public class AutorMap : IEntityTypeConfiguration<Autor>
    {
        public void Configure(EntityTypeBuilder<Autor> builder)
        {
            builder.HasKey(a => a.CodAu);

            builder.Property(a => a.CodAu)
                .ValueGeneratedOnAdd();

            builder.HasIndex(a => a.CodAu).IsUnique(); // Índice na pk

            builder.Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(40);

            #region Relacionamento
            builder.HasMany(a => a.Livros) //muitos-para-muitos com Livro através de LivroAutor
                .WithOne(la => la.Autor)
                .HasForeignKey(la => la.AutorCodAu)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
