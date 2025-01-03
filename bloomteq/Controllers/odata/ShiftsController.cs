
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using bloomteq;
using bloomteq.Models;
using Microsoft.AspNetCore.Authorization;

namespace InformationProtocolSubSystem.Api.Controllers.odata
{
    [Route("odata/Shifts")]
    [Authorize]
    public class ShiftsController : ODataController
    {
        private readonly IApplicationDbContext _context;
        IHttpContextAccessor _httpContextAccessor;

        public ShiftsController(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [EnableQuery]
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var userId= _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c=>c.Type=="userId").Value;
            var result = _context.Shifts.Where(s=>s.UserId==userId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Shift shift)
        {
            shift.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            shift.Id = Guid.NewGuid();
            _context.Shifts.Add(shift);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
            return Created(shift);
        }

        

        [HttpPatch]
        [Authorize]
        public IActionResult Patch([FromODataUri] Guid key, [FromBody] Delta<Shift> patch)
        {
            Shift shift = _context.Shifts.Find(key);
            if (shift == null)
            {
                return NotFound();
            }
            patch.Patch(shift);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
            return Updated(shift);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete([FromODataUri] Guid key)
        {
            Shift shift = _context.Shifts.Find(key);
            if (shift == null)
            {
                return NotFound();
            }
            _context.Remove(shift);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
            return NoContent();
        }


    }
}
