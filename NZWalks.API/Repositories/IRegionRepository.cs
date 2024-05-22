using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IRegionRepository
    {
        Task<List<Region>>GetAllAsync(); // return list of regions back from the DB.

        Task<Region?>GetByIdAsync(Guid id); // return region specifically by id

        Task<Region>CreateAsync(Region region); // create a new region

        Task<Region?> UpdateAsync(Guid id, Region region); // update a region

        Task<Region?> DeleteAsync(Guid id); // delete a region
    }
}
