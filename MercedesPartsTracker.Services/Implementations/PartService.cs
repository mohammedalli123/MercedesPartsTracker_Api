using MercedesPartsTracker.EntityFramework;
using MercedesPartsTracker.EntityFramework.Models;
using MercedesPartsTracker.EntityFrameworkServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MercedesPartsTracker.EntityFrameworkServices.Implementations
{
    public  class PartService : IPartService
    {
        private readonly DBContext _context;

        public PartService(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync()
        {
            return await _context.Parts.ToListAsync();
        }

        public async Task<Part> GetPartByNumberAsync(string partNumber)
        {
            return await _context.Parts.FirstOrDefaultAsync(p => p.PartNumber == partNumber);
        }

        public async Task<Part> CreatePartAsync(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<bool> UpdatePartAsync(string partNumber, Part part)
        {
            if (partNumber != part.PartNumber) return false;

            _context.Entry(part).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PartExistsAsync(partNumber))
                {
                    return false;
                }
                return false;
            }
        }

        public async Task<bool> DeletePartAsync(string partNumber)
        {
            var part = await _context.Parts.FirstOrDefaultAsync(p => p.PartNumber == partNumber);
            if (part == null) return false;

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PartExistsAsync(string partNumber)
        {
            return await _context.Parts.AnyAsync(e => e.PartNumber == partNumber);
        }
    }
}
