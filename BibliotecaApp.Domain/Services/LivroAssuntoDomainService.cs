using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
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

        public async Task<LivroAssunto> EnsureLivroAssuntoExistsAsync(LivroAssuntoPk pk)
        {
            var existingLivroAssunto = await _livroAssuntoRepository.GetById(pk);
            if (existingLivroAssunto == null)
                throw new NotFoundExceptionLivroAssunto(pk.LivroCodl, pk.AssuntoCodAs);

            _unitOfWork.DataContext.Entry(existingLivroAssunto).State = EntityState.Detached;
            return existingLivroAssunto;
        }
        private async Task EnsureLivroExistsAsync(int livroCodl)
        {
            var livro = await _unitOfWork.LivroRepository.GetById(livroCodl);
            if (livro == null)
                throw new NotFoundExceptionLivro(livroCodl);
        }
        private async Task EnsureAssuntoExistsAsync(int assuntoCodAs)
        {
            var assunto = await _unitOfWork.AssuntoRepository.GetById(assuntoCodAs);
            if (assunto == null)
                throw new NotFoundExceptionAssunto(assuntoCodAs);
        }

        public async override Task<LivroAssunto> AddAsync(LivroAssunto entity)
        {
            await EnsureAssuntoExistsAsync(entity.AssuntoCodAs);
            await EnsureLivroExistsAsync(entity.LivroCodl);

            await ValidateEntity(entity);

            var existingLivroAssunto = await _livroAssuntoRepository.GetById(entity.Pk);
            if (existingLivroAssunto != null)
                throw new RecordAlreadyExistsExceptionLivroAssunto(entity.LivroCodl, entity.AssuntoCodAs);

            await _livroAssuntoRepository.Add(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAssunto> UpdateAsync(LivroAssunto entity)
        {
            await ValidateEntity(entity);
            _unitOfWork.DataContext.Entry(entity).State = EntityState.Detached;
            await _livroAssuntoRepository.Update(entity);
            await _unitOfWork.SaveChanges();
            return entity;
        }

        public async override Task<LivroAssunto> DeleteAsync(LivroAssunto entity)
        {
            var livroAssunto = await EnsureLivroAssuntoExistsAsync(entity.Pk);

            await _livroAssuntoRepository.Delete(livroAssunto);
            await _unitOfWork.SaveChanges();
            return livroAssunto;
        }

        private async Task ValidateEntity(LivroAssunto entity)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            _unitOfWork.DataContext.Entry(entity).State = EntityState.Detached;
        }

        public async override Task<List<LivroAssunto>>? GetAllAsync()
        {
            return (List<LivroAssunto>) await _livroAssuntoRepository.GetByConditionAsync(
                pageSize:10, 
                pageNumber:1 ,  
                predicate:null, 
                orderBy:null, 
                isAscending:true,
                includes: new Expression<Func<LivroAssunto, object>>[]
                {
                    la => la.Livro,
                    la => la.Assunto
                },
                cancellationToken:default);
        }

        public async override Task<LivroAssunto>? GetByIdAsync(LivroAssuntoPk id)
        {
            var result = await _livroAssuntoRepository.GetByConditionAsync(
                pageSize: 10,
                pageNumber: 1,
                predicate:
                    la => la.LivroCodl == id.LivroCodl &&
                    la.AssuntoCodAs == id.AssuntoCodAs
                ,
                orderBy: null,
                isAscending: true,
                includes: new Expression<Func<LivroAssunto, object>>[]
                {
                    la => la.Livro,
                    la => la.Assunto
                },
                cancellationToken: default);

            return result.FirstOrDefault();
        }

    }
}
