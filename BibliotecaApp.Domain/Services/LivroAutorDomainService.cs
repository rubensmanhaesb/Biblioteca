using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using FluentValidation;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Services
{
    public class LivroAutorDomainService : BaseDomainService<LivroAutor, LivroAutorPk>, ILivroAutorDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILivroAutorRepository _livroAutorRepository;
        private readonly IValidator<LivroAutor> _validator;

        public LivroAutorDomainService(IUnitOfWork unitOfWork, IValidator<LivroAutor> validator) : base(unitOfWork.LivroAutorRepository!)
        {
            _unitOfWork = unitOfWork;
            _livroAutorRepository = unitOfWork.LivroAutorRepository!;
            _validator = validator;
        }

        public async override Task<LivroAutor> AddAsync(LivroAutor entity)
        {
            await ValidateEntityAsync(entity);

            var existingLivroAutor = await _livroAutorRepository.GetById(entity.Pk);
            if (existingLivroAutor != null)
                throw new RecordAlreadyExistsExceptionLivroAutor(entity.LivroCodl, entity.AutorCodAu);

            await _unitOfWork.LivroAutorRepository.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAutor> UpdateAsync(LivroAutor entity)
        {
            await ValidateEntityAsync(entity);

            await _unitOfWork.LivroAutorRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAutor> DeleteAsync(LivroAutor entity)
        {
            var existingLivroAutor = await _livroAutorRepository.GetById(entity.Pk);
            if (existingLivroAutor == null)
                throw new NotFoundExceptionLivroAutor(entity.LivroCodl, entity.AutorCodAu);

            await _unitOfWork.LivroAutorRepository.Delete(existingLivroAutor);
            await _unitOfWork.SaveChanges();
            return existingLivroAutor;
        }

        private async Task ValidateEntityAsync(LivroAutor entity)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}
