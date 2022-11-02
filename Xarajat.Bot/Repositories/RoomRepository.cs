using Microsoft.EntityFrameworkCore;
using Xarajat.Data.Context;
using Xarajat.Data.Enttities;

namespace Xarajat.Bot.Repositories;

public class RoomRepository
{
    private readonly XarajatDbContext _context;

    public RoomRepository(XarajatDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetRoomById(int roomId)
    {
        return await _context.Rooms.FindAsync(roomId);
    }

    public async Task<Room?> GetRoomByName(string roomName)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Name == roomName);
    }

    public async Task<Room?> GetRoomByKey(string key)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Key == key);
    }

    public async Task AddRoomAsync(Room room)
    {
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoom(Room room)
    {
        _context.Update(room);
        await _context.SaveChangesAsync();
    }
}