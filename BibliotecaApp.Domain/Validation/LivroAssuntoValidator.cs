using BibliotecaApp.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Validation
{
    public class LivroAssuntoValidator : AbstractValidator<LivroAssunto>
    {
        private LivroAssuntoPk livroAssuntoPk;
        public LivroAssuntoValidator()
        {
            ConfigureRules();
        }

        private void ConfigureRules()
        {
            RuleFor(x => x.LivroCodl)
                .NotEmpty().WithMessage("O Id livro é obrigatório.");
            
            RuleFor(x => x.AssuntoCodAs)
                .NotEmpty().WithMessage("O Id assunto é obrigatório.");

        }
    }
}
