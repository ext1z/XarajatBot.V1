namespace Xarajat.Data.Enttities;

public class Outlay
{
    public int Id { get; set; }
    public string Description { get; set; }
    public int Cost { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
    public int? RoomId { get; set; }
    public virtual Room Room { get; set; }

    public string ProductInfo => $"{User.FullName} \n {Description} - {Cost}";

}
