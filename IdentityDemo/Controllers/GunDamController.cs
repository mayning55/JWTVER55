using ClassLibrary;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class GunDamController : ControllerBase
    {
        private readonly EFCoreDBContext _dbContext;

        public GunDamController(EFCoreDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        private bool GunDamExisits(long id)
        {
            return _dbContext.GunDams.Any(x=>x.Id == id);
        }

        [HttpGet]
        public async  Task<ActionResult> GetGunDams()
        {
            var gunDam = await _dbContext.GunDams.ToListAsync();
            return Ok(gunDam);
        }

        [HttpGet]
        public async Task<ActionResult<GunDam>> GetGunDamById(long id)
        {
            var gunDam = await _dbContext.GunDams.FirstOrDefaultAsync(x => x.Id == id);
            return gunDam == null? NotFound($"Unable to find {id}"): Ok(gunDam);
        }
        [HttpDelete]
        public async Task<ActionResult<GunDam>> DeleteGunDam(long id)
        {
            var gunDam = _dbContext.GunDams.Find(id);
            if (gunDam == null)
            {
                return NotFound("");
            }
            _dbContext.Remove(gunDam);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> AddGunDam(GunDam gunDam)
        {
            var itm = new GunDam
            {
                Title = gunDam.Title,
                PubDate = gunDam.PubDate,
                Price =  gunDam.Price,
            };
            _dbContext.GunDams.Add(itm);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGunDamById),new {id = gunDam.Id},gunDam);
        }
        [HttpPut]
        public async Task<ActionResult> UpdatePrice(long id, double price)
        {
            var gunDam = _dbContext.GunDams.Find(id);
            if (gunDam == null)
            {
                return NotFound("");
            }
            gunDam.Price = price;
            _dbContext.GunDams.Update(gunDam);
            await _dbContext.SaveChangesAsync();
            return Ok();

        }
        [HttpPut]
        public async Task<ActionResult> UpdateGunDam(long id,GunDam gunDam)
        {
            if(id != gunDam.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(gunDam).State=EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!GunDamExisits(id))
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
    }
}
