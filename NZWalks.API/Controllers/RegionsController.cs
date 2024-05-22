using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:1234/api/Regions
    [Route("api/[controller]")] // Whenever an user enters this route it will be pointed to the regionscontroller.
    [ApiController] // Will tell this controller that it's for API use. Validates the model state already
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        // GET ALL REGIONS
        // GET: https://localhost:portnumber/api/Regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get Data From Database - Domain models
            //var regionsDomain = await dbContext.Regions.ToListAsync();

            //Call interface instead
            var regionsDomain = await regionRepository.GetAllAsync();

            // Map Domain Models to DTO's
            /*var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto() { 
                    
                    Id = regionDomain.Id, 
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl });

            }*/

            // Using Auto-Mapper. Map Domain Models to DTO's
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            // Return DTO's
            return Ok(regionsDto);
        }

        // GET SINGLE REGION (Get Region By ID)
        // GET: https://localhost:portnumbers/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task <IActionResult> GetById([FromRoute]Guid id) {
            //var region = dbContext.Regions.Find(id);
            // Get Region Domain Model From Database
            //var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            

            // get through repository
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if(regionDomain == null)
            {
                return NotFound();
            }

            // Map Region Domain Model to Region DTO
            /*var regionDto = new RegionDto
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl
            };*/

            // Using Auto-Mapper. Map Domain Models to DTO's
            var regionDto = mapper.Map<List<RegionDto>>(regionDomain);

            // return DTO back to client
            return Ok(regionDto);
        }

        // POST To Create New Region
        // POST: https://localhost:portnumber/api/regions
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task <IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            if (ModelState.IsValid)
            {
                // Map DTO Domain Model
                /*var regionDomainModel = new Region
                {
                    Code = addRegionRequestDto.Code,
                    Name = addRegionRequestDto.Name,
                    RegionImageUrl = addRegionRequestDto.RegionImageUrl
                };*/

                // Using Auto-Mapper. Map Domain Models to DTO's
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);
                // Use Domain Model to create Region using DB context.
                // await dbContext.Regions.AddAsync(regionDomainModel);
                // await dbContext.SaveChangesAsync(); // Saves the changes to the database. It will be reflected in the SQL server.

                // Create region using repository
                await regionRepository.CreateAsync(regionDomainModel);

                // Map Domain model back to DTO
                /*var regionDto = new RegionDto
                {
                    Id = regionDomainModel.Id,
                    Code = regionDomainModel.Code,
                    Name = regionDomainModel.Name,
                    RegionImageUrl = regionDomainModel.RegionImageUrl
                };*/

                // Map Domain model back to DTO using Auto-Mapper
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
            }
            return BadRequest();
            
        }

        // Update region
        // PUT: https://localhost:portnumber/api/regions/{id}
        // Doing it like {id:Guid} grants type safety
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task <IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Check if region exists (non repository way)
            // var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            // Map DTO to Domain Model
            /*var regionDomainModel = new Region
            {
                Code = updateRegionRequestDto.Code,
                Name = updateRegionRequestDto.Name,
                RegionImageUrl= updateRegionRequestDto.RegionImageUrl
            };*/

            // Using Auto-Mapper. Map Domain Models to DTO's
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
            await regionRepository.UpdateAsync(id, regionDomainModel);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            await dbContext.SaveChangesAsync();

            // Convert Domain Model to DTO
            /*var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl

            };*/
            // Using Auto-Mapper. Convert Domain model to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        }

        // Delete region
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Authorize(Roles = "Writer")]
        public async Task <IActionResult> Delete([FromRoute] Guid id) {
            // Non repository way
            //var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }

            // Using Auto-Mapper. Delete DTO's
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
    
            // Delete region non repository way
            //dbContext.Regions.Remove(regionDomainModel); 
            //await dbContext.SaveChangesAsync();

            return Ok(regionDto);
        }
    }
}
