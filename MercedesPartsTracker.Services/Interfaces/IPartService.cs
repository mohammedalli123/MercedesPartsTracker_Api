using MercedesPartsTracker.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercedesPartsTracker.EntityFrameworkServices.Interfaces
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllPartsAsync();
        Task<Part> GetPartByNumberAsync(string partNumber);
        Task<Part> CreatePartAsync(Part part);
        Task<bool> UpdatePartAsync(string partNumber, Part part);
        Task<bool> DeletePartAsync(string partNumber);
        Task<bool> PartExistsAsync(string partNumber);
    }
}
