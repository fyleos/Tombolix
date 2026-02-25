using Microsoft.EntityFrameworkCore;
using TradeUp.Server.Data;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Services
{
    public class SharedUserOptionService
    {
        private readonly ApplicationDbContext _context;
        //private readonly HttpClient _http;

        public SharedUserOptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SharedUserOption?> GetSharedUserOptionByUserIdAsync(string userId)
        {
            return await _context.SharedUserOptions.FirstOrDefaultAsync(suo => suo.UserId == userId);
        }

    }

}
