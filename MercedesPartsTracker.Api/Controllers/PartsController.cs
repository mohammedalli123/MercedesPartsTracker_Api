using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercedesPartsTracker.EntityFramework;
using MercedesPartsTracker.EntityFramework.Models;
using MercedesPartsTracker.EntityFrameworkServices.Interfaces;

namespace MercedesPartsTracker.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            _partService = partService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts()
        {
            return Ok(await _partService.GetAllPartsAsync());
        }

        [HttpGet("{partNumber}")]
        public async Task<ActionResult<Part>> GetPart(string partNumber)
        {
            var part = await _partService.GetPartByNumberAsync(partNumber);
            if (part == null)
            {
                return NotFound();
            }
            return Ok(part);
        }

        [HttpPost]
        public async Task<ActionResult<Part>> CreatePart(Part part)
        {
            if (await _partService.PartExistsAsync(part.PartNumber))
            {
                return BadRequest($"Part with PartNumber '{part.PartNumber}' already exists.");
            }
            if (part.QuantityOnHand < 0) {
                return BadRequest($"Parts cannot have a qty < 0.");
            }

            var createdPart = await _partService.CreatePartAsync(part);
            return CreatedAtAction(nameof(GetPart), new { partNumber = createdPart.PartNumber }, createdPart);
        }

        [HttpPut("{partNumber}")]
        public async Task<IActionResult> UpdatePart(string partNumber, Part part)
        {
            if (part.QuantityOnHand < 0)
            {
                return BadRequest($"Parts cannot have a qty < 0.");
            }

            try
            {
                var success = await _partService.UpdatePartAsync(partNumber, part);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _partService.PartExistsAsync(partNumber))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpDelete("{partNumber}")]
        public async Task<IActionResult> DeletePart(string partNumber)
        {
            var success = await _partService.DeletePartAsync(partNumber);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
