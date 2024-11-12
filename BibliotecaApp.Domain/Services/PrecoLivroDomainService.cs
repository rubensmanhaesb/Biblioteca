using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BibliotecaApp.Domain.Services
{
    public class PrecoLivroDomainService : BaseDomainService<PrecoLivro, int>, IPrecoLivroDomainService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrecoLivroRepository _precoLivroRepository;

        public PrecoLivroDomainService(IUnitOfWork unitOfWork) : base(unitOfWork.PrecoLivroRepository!)
        {
            _unitOfWork = unitOfWork;
            _precoLivroRepository = unitOfWork.PrecoLivroRepository!;
        }
        private async Task ValidateAndThrowAsync(TipoOperacao tipoOperacao, PrecoLivro entity)
        {
            var validator = new PrecoLivroValidator(tipoOperacao);
            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

        }
        private async Task<PrecoLivro>? EnsureEntityExistsAsync(PrecoLivro entity)
        {
            var precoLivro = await _precoLivroRepository.GetById(entity.Codp);
            if (precoLivro == null)
                throw new NotFoundExceptionPrecoLivro(entity.Codp);
            
            return precoLivro;
        }

        public async override Task<PrecoLivro> AddAsync(PrecoLivro entity)
        {
            await ValidateAndThrowAsync(TipoOperacao.Inclusao, entity);

            var existingPrecoLivro = await _precoLivroRepository.GetById(entity.Codp);
            if (existingPrecoLivro != null)
                throw new RecordAlreadyExistsExceptionPrecoLivro(entity.Codp);


            await _unitOfWork.PrecoLivroRepository!.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<PrecoLivro> UpdateAsync(PrecoLivro entity)
        {
            await ValidateAndThrowAsync(TipoOperacao.Alteracao, entity);

            await EnsureEntityExistsAsync(entity);

            await _unitOfWork.PrecoLivroRepository!.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<PrecoLivro> DeleteAsync(PrecoLivro entity)
        {
            await ValidateAndThrowAsync(TipoOperacao.Delecao, entity);
            
            var precoLivro = await EnsureEntityExistsAsync(entity);

            await _unitOfWork.PrecoLivroRepository!.Delete(precoLivro);
            await _unitOfWork.SaveChanges();

            return precoLivro;
        }

        public async Task<IEnumerable<PrecoLivro?>> GetByConditionAsync(int? pageSize = null, int? pageNumber = null, Expression<Func<PrecoLivro, bool>> predicate = null, Expression<Func<PrecoLivro, object>>[] orderBy = null, bool isAscending = true, Expression<Func<PrecoLivro, object>>[] includes = null, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.PrecoLivroRepository!.GetByConditionAsync(pageSize, pageNumber, predicate, orderBy, isAscending, includes, cancellationToken);
        }
    }
}