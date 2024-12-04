using BibliotecaApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public class LivroAssuntoResponseDto
    {
        public int LivroCodl { get; set; }
        public int AssuntoCodAs { get; set; }

        #region Relacionamentos
        public LivroResponseDto Livro { get; set; }
        public AssuntoResponseDto Assunto { get; set; }
        #endregion
    }
}
