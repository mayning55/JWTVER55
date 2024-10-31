using ClassLibrary;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly EFCoreDBContext _dbContext;
        public PersonController(EFCoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        private bool PersonExists(long id)
        {
            return _dbContext.Persons.Any(x=>x.Id == id);
        }
        [HttpGet]
        public async Task<ActionResult> GetPersons()
        {
            var person = await _dbContext.Persons.ToListAsync();
            return Ok(person);
        }
        [HttpGet]
        public async Task<ActionResult<Person>> GetPersonById(long id)
        {
            var person = await _dbContext.Persons.FindAsync(id);
            return person == null ? NotFound() : person;
        }
    
        [HttpDelete]
        public async Task<ActionResult> DeletePerson(long id)
        {
            var person = await _dbContext.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            _dbContext.Persons.Remove(person);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult<Person>> AddPerson(Person person)
        {
            var item = new Person
            {
                Number = person.Number,
                FirstName = person.FirstName,
            };
            _dbContext.Persons.Add(item);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonById),new { id = person.Id }, person);
        }
        [HttpPut]
        public async Task<ActionResult> Updaeperson(long id,Person person)
        {
            if(id != person.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(person).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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
