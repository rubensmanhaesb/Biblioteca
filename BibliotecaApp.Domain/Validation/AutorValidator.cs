using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Validation
{
    public class AutorValidator : AbstractValidator<Autor>
    {

        public AutorValidator(TipoOperacao tipoOperacao)
        {
            ConfigureRules(tipoOperacao);
        }


        private void ConfigureRules(TipoOperacao tipoOperacao)
        {
            if (tipoOperacao == TipoOperacao.Inclusao || tipoOperacao == TipoOperacao.Alteracao)
            {
                RuleFor(x => x.Nome)
                    .NotEmpty().WithMessage("O nome do autor é obrigatório.")
                    .MaximumLength(40).WithMessage("O nome deve ter no máximo 40 caracteres");
            }

            switch (tipoOperacao)
            {
                case TipoOperacao.Inclusao:
                    RuleFor(x => x.CodAu)
                        .Equal(0).WithMessage("Código do autor não deve ser informado na inclusão.");
                    break;

                case TipoOperacao.Alteracao:
                    RuleFor(x => x.CodAu)
                        .GreaterThan(0).WithMessage("Código do autor deve ser informado na alteração.");
                    break;

                case TipoOperacao.Delecao:
                    RuleFor(x => x.CodAu)
                        .GreaterThan(0).WithMessage("Código do autor deve ser informado na exclusão.");
                    break;

            }


        }
    }
}
