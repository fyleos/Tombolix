using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TradeUp.Server.Data;
using TradeUp.Server.Data.Migrations;
using TradeUp.Server.Models;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class FeaturesController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeaturesController( ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ObservableCollection<FeatureDto>>> GetFeatures()
        {
            var applicationFeatures = await _context.ApplicationFeatures.ToListAsync();

            if(applicationFeatures is null)
            {
                return NotFound();
            }

            ObservableCollection<FeatureDto> features = new ObservableCollection<FeatureDto>();
            foreach (var feature in applicationFeatures) {
                features.Add(new FeatureDto { 
                        Id = feature.Id, 
                    Name = feature.Name, 
                    ShortDescription = feature.ShortDescription,
                    EnabledRoles = feature.EnabledGroups, 
                    IsUserAuthorized = true });
            }

            return Ok(features);
        }

        [HttpPut("add")]
        public async Task<ActionResult> AddFeature([FromBody] FeatureDto feature)
        { 
            if(feature is null)
            {
                return BadRequest();
            }

            var features = await _context.ApplicationFeatures.ToListAsync();

            if(features.Any(f => f.Name == feature.Name))
            {
                return BadRequest();
            }

            ApplicationFeature newFeature = new ApplicationFeature
            {
                Name = feature.Name,
                ShortDescription = feature.ShortDescription,
                EnabledGroups = feature.EnabledRoles
            };

            _context.ApplicationFeatures.Add(newFeature);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(newFeature.Name))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPut("update")]
        public async Task<ActionResult> UpdateFeature([FromBody] FeatureDto feature)
        {
            if (feature is null)
            {
                return BadRequest();
            }

            var features = await _context.ApplicationFeatures.ToListAsync();


            ApplicationFeature? featureToUpdate = features.FirstOrDefault(f => f.Id == feature.Id);
            
            if (featureToUpdate is null)
            {
                return BadRequest();
            }

            featureToUpdate.Name = feature.Name;
            featureToUpdate.ShortDescription = feature.ShortDescription;
            featureToUpdate.EnabledGroups = feature.EnabledRoles;

            _context.Update(featureToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(featureToUpdate.Name))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /***
         * 
         * 
         * public async Task<ActionResult> SetUserOption([FromRoute]string id,[FromBody] SharedUserOption sharedUserOption)
        {
            if (id != sharedUserOption.UserId)
            {
                return BadRequest();
            }

            var sharedUserOptions = await _context.SharedUserOptions.ToListAsync();
            if(sharedUserOptions is not null)
            {
                var dbUserOption = sharedUserOptions.FirstOrDefault(option => option.UserId == id);
                if (dbUserOption is not null)
                {
                    dbUserOption.CurrentTheme = sharedUserOption.CurrentTheme;
                    _context.Entry(dbUserOption).State = EntityState.Modified;
                }
                else
                {
                    _context.SharedUserOptions.Add(sharedUserOption);
                }
            }
            else
            {
                _context.SharedUserOptions.Add(sharedUserOption);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SharedUserOptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
         * 
         * 
         * */


        private bool FeatureExists(string name)
        {
            return _context.ApplicationFeatures.Any(f => f.Name == name);
        }
    }
}
