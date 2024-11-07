using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Services
{
    public class LivroDomainService : BaseDomainService<Livro, int>, ILivroDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILivroRepository _livroRepository;

        public LivroDomainService(IUnitOfWork unitOfWork) : base(unitOfWork.LivroRepository!)
        {
            _unitOfWork = unitOfWork;
            _livroRepository = unitOfWork.LivroRepository!;
        }

        private async Task ValidateAndThrowAsync(TipoOperacao tipoOperacao, Livro entity)
        {
            var validator = new LivroValidator(tipoOperacao);
            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        private async Task EnsureEntityExistsAsync(int id)
        {
            var entity = await _livroRepository.GetById(id);
            if (entity == null)
                throw new NotFoundExceptionLivro(id);

            _unitOfWork.DataContext.Entry(entity).State = EntityState.Detached; 
        }

        public async override Task<Livro> AddAsync(Livro entity)
        {
            await ValidateAndThrowAsync(TipoOperacao.Inclusao, entity);
            await _unitOfWork.LivroRepository.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Livro> UpdateAsync(Livro entity)
        {
            await ValidateAndThrowAsync(TipoOperacao.Alteracao, entity);
            await EnsureEntityExistsAsync(entity.Codl);

            await _unitOfWork.LivroRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Livro> DeleteAsync(Livro entity)
        {
            var livro = await _livroRepository.GetById(entity.Codl);
            if (livro == null)
                throw new NotFoundExceptionLivro(entity.Codl);
            
            await ValidateAndThrowAsync(TipoOperacao.Delecao, livro);
            await _unitOfWork.LivroRepository.Delete(livro);
            await _unitOfWork.SaveChanges();
            return livro;
        }
    }
}
