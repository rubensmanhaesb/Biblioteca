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
    public class AssuntoMap : IEntityTypeConfiguration<Assunto>
    {
        public void Configure(EntityTypeBuilder<Assunto> builder)
        {
            builder.HasKey(a => a.CodAs);

            builder.Property(a => a.CodAs)
                .ValueGeneratedOnAdd();

            builder.HasIndex(a => a.CodAs).IsUnique(); // Índice na chave pk

            builder.Property(a => a.Descricao)
                .HasMaxLength(20);

            #region Relacionamentos
            builder.HasMany(a => a.Livros) //muitos-para-muitos com Livro através de LivroAssunto
                .WithOne(la => la.Assunto)
                .HasForeignKey(la => la.AssuntoCodAs)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
