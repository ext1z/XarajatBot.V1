namespace Xarajat.Data.Enttities;


// Second Way how to write table Configuration in ContextConifuration.
public class Room
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public RoomStatus Status { get; set; }
    public string? Key { get; set; }

    public virtual List<User>? Users { get; set; }
    public virtual List<Outlay>? Outlays { get; set; }
}
