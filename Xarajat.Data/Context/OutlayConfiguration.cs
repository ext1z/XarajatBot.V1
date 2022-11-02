using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xarajat.Data.Enttities;

namespace Xarajat.Data.Context;


// Table Configuration second way. With configuration class
public class OutlayConfiguration
{
    public static void Configure(EntityTypeBuilder<Outlay> builder)
    {
        builder.ToTable("outlays")
            .HasKey(outlay => outlay.Id);

        builder.Property(property => property.Description)
            .HasColumnType("nvarchar(75)")
            .IsRequired(false);

        builder.Property(property => property.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(property => property.RoomId)
            .HasColumnName("room_id")
            .IsRequired();

        // One to One and One to Many (ForUsersOfOutlays)
        builder.HasOne(outlay => outlay.User)
            .WithMany(user => user.Outlays)
            .HasForeignKey(outlay => outlay.UserId);

        // One to One and One to Many (ForRoomOfOutlays)
        builder.HasOne(outlay => outlay.Room)
            .WithMany(room => room.Outlays)
            .HasForeignKey(outlay => outlay.RoomId);

        builder.Ignore(outlay => outlay.ProductInfo);
    }
}
