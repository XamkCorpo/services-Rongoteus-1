using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProductApi.Common;
using ProductApi.Mappings;
using ProductApi.Models;
using ProductApi.Models.Dtos;
using ProductApi.Repositories;

namespace ProductApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repo, ILogger<CategoryService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result<List<CategoryResponse>>> GetAllAsync()
        {
            try
            {
                List<Category> category = await _repo.GetAllAsync();
                List<CategoryResponse> response = category.Select(p => p.ToResponse()).ToList();
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe kategorian haussa");
                return Result.Failure<List<CategoryResponse>>("Kategorian haku epäonnistui");
            }
        }

        public async Task<Result<CategoryResponse>> GetByIdAsync(int id)
        {
            try
            {
                Category? category = await _repo.GetByIdAsync(id);

                if (category == null)
                    return Result.Failure<CategoryResponse>($"Kategoria {id} ei löytynyt");

                return Result.Success(category.ToResponse());
            }
            catch (Exception ex)
            {
                
                return Result.Failure<CategoryResponse>("Kategorian haku");
            }
        }

        public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            try
            {
                Category category = request.ToEntity();
                Category created = await _repo.AddAsync(category);
                return Result.Success(created.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe tuotteen luomisessa: {ProductName}", request.Name);
                return Result.Failure<CategoryResponse>("Tuotteen luominen epäonnistui");
            }
        }

        public async Task<Result<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            try
            {
                Category? existing = await _repo.GetByIdAsync(id);

                if (existing == null)
                    return Result.Failure<CategoryResponse>($"Kategoriaa {id} ei löytynyt");

                request.UpdateEntity(existing);
                await _repo.UpdateAsync(existing);
                return Result.Success(existing.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe kategorian päivittämisessä: {CategoryId}", id);
                return Result.Failure<CategoryResponse>("Kategorian päivittäminen epäonnistui");
            }
        }

        public async Task<Result> DeleteAsync(int id)
        {
            try
            {
                bool deleted = await _repo.DeleteAsync(id);

                if (!deleted)
                    return Result.Failure($"Kategorian {id} ei löytynyt");

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe kategorian poistamisessa: {ProductId}", id);
                return Result.Failure("Kategorian poistaminen epäonnistui");
            }
        }
    }
}
