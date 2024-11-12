using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using FluentValidation;

namespace BibliotecaApp.Domain.Validation
{
    public class LivroValidator : AbstractValidator<Livro>
    {
        private int _currentLivroId;

        public LivroValidator(TipoOperacao tipoOperacao)
        {
            ConfigureRules(tipoOperacao);
        }


        private void ConfigureRules(TipoOperacao tipoOperacao)
        {

            if (tipoOperacao == TipoOperacao.Inclusao)
                    RuleFor(x => x.Codl)
                        .Equal(0).WithMessage("Código do livro não deve ser informado na inclusão.");

            if (tipoOperacao == TipoOperacao.Inclusao || tipoOperacao == TipoOperacao.Alteracao)
                ConfigureGeneralRules();

            if (tipoOperacao == TipoOperacao.Delecao || tipoOperacao == TipoOperacao.Alteracao)
                RuleFor(x => x.Codl)
                .NotEmpty().WithMessage("O código livro é obrigatório.")
                .GreaterThan(0).WithMessage("O do livro deve ser maior que zero.");
        }

        private void ConfigureGeneralRules()
        {

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório.")
                .MaximumLength(40).WithMessage("O título deve ter no máximo 40 caracteres");

            RuleFor(x => x.Editora)
                .NotEmpty().WithMessage("A Editora do livro é obrigatória.")
                .MaximumLength(40).WithMessage("A Editora deve ter no máximo 40 caracteres");

            RuleFor(x => x.Edicao)
                .NotEmpty().WithMessage("A Edição do livro é obrigatória.")
                .GreaterThan(0).WithMessage("A Edição não pode ser zero ou negativa.");

            RuleFor(x => x.AnoPublicacao)
                .NotEmpty().WithMessage("A Publicação do livro é obrigatória.")
                .Length(4).WithMessage("A Publicação deve ter exatamente 4 caracteres.");
        }
    }
}
