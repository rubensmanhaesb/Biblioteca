using BibliotecaApp.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Validation
{
    public class LivroAutorValidator : AbstractValidator<LivroAutor>        
    {
        private LivroAutorPk _livroAssuntoPk;

        public LivroAutorValidator()
        {
            ConfigureRules();
        }

        private void ConfigureRules()
        {
            RuleFor(x => x.LivroCodl)
                .NotEmpty().WithMessage("O id do livro é obrigatório")
                .Must(id => id != 0).WithMessage("O id não pode ser igual ao valor padrão.");

            RuleFor(x => x.AutorCodAu)
                .NotEmpty().WithMessage("O id do autor é obrigatório")
                .When(x => _livroAssuntoPk != null)
                .Must(id => id != 0).WithMessage("O id não pode ser igual ao valor padrão.");

        }

    }
}
