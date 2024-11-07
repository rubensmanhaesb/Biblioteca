using BibliotecaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Infra.Data.Mappings
{
    public class PrecoLivroMap : IEntityTypeConfiguration<PrecoLivro>
    {
        public void Configure(EntityTypeBuilder<PrecoLivro> builder)
        {
            // Define a chave primária
            builder.HasKey(p => p.Codp);

            
            builder.HasOne(p => p.Livro)
                .WithMany(l => l.Precos)
                .HasForeignKey(p => p.LivroCodl);

            
            builder.Property(p => p.TipoCompra)
                .HasConversion<int>() 
                .IsRequired(); 

            
            builder.Property(p => p.Valor)
                .HasColumnType("decimal(18,2)") 
                .IsRequired(); 

            
            builder.HasIndex(p => new { p.LivroCodl, p.TipoCompra })
                .IsUnique(); // Opcional: garante que cada Livro tenha apenas um preço por TipoCompra
        }
    }
}
