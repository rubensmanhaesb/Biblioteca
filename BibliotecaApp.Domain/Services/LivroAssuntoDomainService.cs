using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using FluentValidation;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Services
{
    public class LivroAssuntoDomainService : BaseDomainService<LivroAssunto, LivroAssuntoPk>, ILivroAssuntoDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILivroAssuntoRepository _livroAssuntoRepository;
        private readonly IValidator<LivroAssunto> _validator;

        public LivroAssuntoDomainService(IUnitOfWork unitOfWork, IValidator<LivroAssunto> validator) : base(unitOfWork.LivroAssuntoRepository!)
        {
            _unitOfWork = unitOfWork;
            _livroAssuntoRepository = unitOfWork.LivroAssuntoRepository!;
            _validator = validator;
        }

        public async override Task<LivroAssunto> AddAsync(LivroAssunto entity)
        {
            await ValidateEntity(entity);

            var existingLivroAssunto = await _livroAssuntoRepository.GetById(entity.Pk());
            if (existingLivroAssunto != null)
                throw new RecordAlreadyExistsExceptionLivroAssunto(entity.LivroCodl, entity.AssuntoCodAs);

            await _livroAssuntoRepository.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAssunto> UpdateAsync(LivroAssunto entity)
        {
            await ValidateEntity(entity);
            await _livroAssuntoRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAssunto> DeleteAsync(LivroAssunto entity)
        {
            var pk = new LivroAssuntoPk { AssuntoCodAs = entity.AssuntoCodAs, LivroCodl = entity.LivroCodl };
            var livroAssunto = await _livroAssuntoRepository.GetById(pk);

            if (livroAssunto == null)
                throw new NotFoundExceptionLivroAssunto(entity.LivroCodl, entity.AssuntoCodAs);

            await _livroAssuntoRepository.Delete(livroAssunto);
            await _unitOfWork.SaveChanges();
            return livroAssunto;
        }

        private async Task ValidateEntity(LivroAssunto entity)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}
