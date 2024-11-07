using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Services
{
    public class AutorDomainService : BaseDomainService<Autor, int>, IAutorDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAutorRepository _autorRepository;

        public AutorDomainService(IUnitOfWork unitOfWork) : base(unitOfWork.AutorRepository!)
        {
            _unitOfWork = unitOfWork;
            _autorRepository = unitOfWork.AutorRepository!;
        }

        private async Task ValidateEntityAsync(TipoOperacao tipoOperacao, Autor entity)
        {
            var validator = new AutorValidator(tipoOperacao);
            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        public async override Task<Autor> AddAsync(Autor entity)
        {
            await ValidateEntityAsync(TipoOperacao.Inclusao, entity);
            await _unitOfWork.AutorRepository!.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Autor> UpdateAsync(Autor entity)
        {
            await ValidateEntityAsync(TipoOperacao.Alteracao, entity);

            var autor = await _autorRepository.GetById(entity.CodAu );
            if (autor == null)
                throw new NotFoundExceptionAutor(entity.CodAu);
            _unitOfWork.DataContext.Entry(autor).State = EntityState.Detached;

            await _unitOfWork.AutorRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Autor> DeleteAsync(Autor entity)
        {
            var autor = await _autorRepository.GetById(entity.CodAu);
            if (autor == null)
                throw new NotFoundExceptionAutor(entity.CodAu);
            await ValidateEntityAsync(TipoOperacao.Delecao, autor);

            await _unitOfWork.AutorRepository.Delete(autor);
            await _unitOfWork.SaveChanges();
            return autor;
        }
    }
}
