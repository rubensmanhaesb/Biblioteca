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
    public class AssuntoValidator : AbstractValidator<Assunto>
    {

        public AssuntoValidator(TipoOperacao tipoOperacao)
        {
            ConfigureRules(tipoOperacao);
        }


        private void ConfigureRules(TipoOperacao tipoOperacao)
        {
            if (tipoOperacao == TipoOperacao.Inclusao || tipoOperacao == TipoOperacao.Alteracao)
            {
                RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição do assunto é obrigatória.")
                .MaximumLength(20).WithMessage("A aescrição deve ter no máximo 20 caracteres");

            }
            switch (tipoOperacao)
            {
                case TipoOperacao.Inclusao:
                    RuleFor(x => x.CodAs)
                        .Equal(0).WithMessage("Código do assunto não deve ser informado na inclusão.");
                    break;

                case TipoOperacao.Alteracao:
                    RuleFor(x => x.CodAs )
                        .GreaterThan(0).WithMessage("Código do assunto deve ser informado na alteração.");
                    break;

                case TipoOperacao.Delecao:
                    RuleFor(x => x.CodAs)
                        .GreaterThan(0).WithMessage("Código do assunto deve ser informado na exclusão.");
                    break;

            }

        }

    }
}
