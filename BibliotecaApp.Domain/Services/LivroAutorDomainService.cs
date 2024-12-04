using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
        private async Task EnsureLivroExistsAsync(int livroCodl)
        {
            var livro = await _unitOfWork.LivroRepository.GetById(livroCodl);
            if (livro == null)
                throw new NotFoundExceptionLivro(livroCodl);
        }

        private async Task EnsureAutorExistsAsync(int autorCodAu)
        {
            var autor = await _unitOfWork.AutorRepository.GetById(autorCodAu);
            if (autor == null)
                throw new NotFoundExceptionAutor(autorCodAu);
        }
        private async Task<LivroAutor> EnsureLivroAutorExistsAsync(LivroAutorPk pk)
        {
            var existingLivroAutor = await _livroAutorRepository.GetById(pk);
            if (existingLivroAutor == null)
                throw new NotFoundExceptionLivroAutor(pk.LivroCodl, pk.AutorCodAu);

            _unitOfWork.DataContext.Entry(existingLivroAutor).State = EntityState.Detached;
            return existingLivroAutor;
        }


        public async override Task<LivroAutor> AddAsync(LivroAutor entity)
        {
            await EnsureAutorExistsAsync(entity.AutorCodAu);
            await EnsureLivroExistsAsync(entity.LivroCodl);
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
            await EnsureLivroAutorExistsAsync(entity.Pk);
            await ValidateEntityAsync(entity);

            await _unitOfWork.LivroAutorRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAutor> DeleteAsync(LivroAutor entity)
        {
            var existingLivroAutor = await EnsureLivroAutorExistsAsync(entity.Pk);

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

        public async override Task<List<LivroAutor>>? GetAllAsync()
        {
            return (List<LivroAutor>)await _livroAutorRepository.GetByConditionAsync(
                pageSize: 10,
                pageNumber: 1,
                predicate: null,
                orderBy: null,
                isAscending: true,
                includes: new Expression<Func<LivroAutor, object>>[]
                {
                    la => la.Livro,
                    la => la.Autor
                },
                cancellationToken: default);
        }

        public async override Task<LivroAutor>? GetByIdAsync(LivroAutorPk id)
        {
            var result = await _livroAutorRepository.GetByConditionAsync(
                pageSize: 10,
                pageNumber: 1,
                predicate:
                    la => la.LivroCodl == id.LivroCodl &&
                    la.AutorCodAu == id.AutorCodAu
                ,
                orderBy: null,
                isAscending: true,
                includes: new Expression<Func<LivroAutor, object>>[]
                {
                    la => la.Livro,
                    la => la.Autor
                },
                cancellationToken: default);

            return result.FirstOrDefault();
        }

    }
}
