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
    public class LivroAssuntoMap : IEntityTypeConfiguration<LivroAssunto>
    {
        public void Configure(EntityTypeBuilder<LivroAssunto> builder)
        {
            builder.HasKey(la => new { la.LivroCodl, la.AssuntoCodAs });

            builder.HasIndex(la => la.LivroCodl); // Índice na FK
            builder.HasIndex(la => la.AssuntoCodAs); // Índice FK

            builder.HasOne(la => la.Livro)
                .WithMany(l => l.Assuntos)
                .HasForeignKey(la => la.LivroCodl)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(la => la.Assunto)
                .WithMany(a => a.Livros)
                .HasForeignKey(la => la.AssuntoCodAs)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
