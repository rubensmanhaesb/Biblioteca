using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using FluentValidation;

namespace BibliotecaApp.Domain.Validation
{
    public class PrecoLivroValidator : AbstractValidator<PrecoLivro>
    {
        public PrecoLivroValidator(TipoOperacao tipoOperacao)
        {
            ConfigureRules(tipoOperacao);
        }

        private void ConfigureRules(TipoOperacao tipoOperacao)
        {
            if (tipoOperacao == TipoOperacao.Inclusao || tipoOperacao == TipoOperacao.Alteracao)
            {
                ConfigureGeneralRules();
            }

            if (tipoOperacao == TipoOperacao.Delecao || tipoOperacao == TipoOperacao.Alteracao)
            {
                RuleFor(x => x.Codp)
                    .NotEmpty().WithMessage("O código do preço do livro é obrigatório.")
                    .GreaterThan(0).WithMessage("O código do preço deve ser maior que zero.");
            }

        }
        private void ConfigureGeneralRules()
        {
            RuleFor(x => x.LivroCodl)
                .NotEmpty().WithMessage("O código do livro é obrigatório.")
                .GreaterThan(0).WithMessage("O código do livro deve ser maior que zero.");

            RuleFor(x => x.TipoCompra)
                .IsInEnum().WithMessage("O tipo de compra é inválido.");

            RuleFor(x => x.Valor)
                .NotEmpty().WithMessage("O valor do livro é obrigatório.")
                .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");
        }
    }
}
