using Xarajat.Data.Context;
using Xarajat.Data.Enttities;

namespace Xarajat.Bot.Repositories;

public class OutlayRepository
{
    private readonly XarajatDbContext _context;

    public OutlayRepository(XarajatDbContext context)
    {
        _context = context;
    }

    public async Task AddOutlayAsync(Outlay outlay)
    {
        await _context.Outlays.AddAsync(outlay);
        await _context.SaveChangesAsync();
    }

    
}
