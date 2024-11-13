using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;


namespace BibliotecaApp.Domain.Services
{
    public class AssuntoDomainService : BaseDomainService<Assunto, int>, IAssuntoDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssuntoRepository _assuntoRepository;

        public AssuntoDomainService(IUnitOfWork unitOfWork ) : base(unitOfWork.AssuntoRepository!)
        {
            _unitOfWork = unitOfWork;
            _assuntoRepository = unitOfWork.AssuntoRepository!;
        }

        private async Task ValidateEntityAsync(TipoOperacao tipoOperacao, Assunto entity)
        {
            var validator = new AssuntoValidator(tipoOperacao);
            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        public async override Task<Assunto> AddAsync(Assunto entity)
        {
            await ValidateEntityAsync(TipoOperacao.Inclusao, entity);
            await _unitOfWork.AssuntoRepository!.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Assunto> UpdateAsync(Assunto entity)
        {
            await ValidateEntityAsync(TipoOperacao.Alteracao, entity);

            var assunto = await _assuntoRepository.GetById(entity.CodAs);
            if (assunto == null)
                throw new NotFoundExceptionAssunto(entity.CodAs);
            _unitOfWork.DataContext.Entry(assunto).State = EntityState.Detached;

            await _unitOfWork.AssuntoRepository!.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<Assunto> DeleteAsync(Assunto entity)
        {

            var assunto = await _assuntoRepository.GetById(entity.CodAs);
            if (assunto == null)
                throw new NotFoundExceptionAssunto(entity.CodAs);
            
            await ValidateEntityAsync(TipoOperacao.Delecao, assunto);
            await _unitOfWork.AssuntoRepository!.Delete(assunto);
            await _unitOfWork.SaveChanges();
            return assunto;
        }

    }
}
