using AutoTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xarajat.Bot.Repositories;
using Xarajat.Data.Context;
using Xarajat.Data.Enttities;
using User = Xarajat.Data.Enttities.User;


namespace Xarajat.Bot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BotController : ControllerBase
{
    private readonly XarajatDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly RoomRepository _roomRepository;
    private readonly OutlayRepository _outlayRepositroy;
    private readonly TelegramBotService _telegramBotService;


    public BotController(XarajatDbContext context ,UserRepository userRepository,
        RoomRepository roomRepository,
        TelegramBotService telegramBotService,
        OutlayRepository outlayRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _outlayRepositroy = outlayRepository;
        _telegramBotService = telegramBotService;
    }


    [HttpGet]
    public IActionResult GetBot() => Ok("Still Working...");

    [HttpPost]
    public async Task GetMessage(Update update)
    {
        if (update.Type is not UpdateType.Message) return;


        var (chatId, message, name) = GetUpdate(update);

        var user = await FilterUser(chatId, name);

        
        if (user.Step == (int)EStepMenu.Menu)
        {
            // Create Room Message Step Change
            if (message == "Create Room")
            {
                user.Step = (int)EStepMenu.AddRoom;
                await _userRepository.Update(user);
                _telegramBotService.SendMessage(user.ChatId, "Enter Room Name");
            }
            // Joint Room Message Step Change
            else if (message == "Join Room")
            {
                user.Step = (int)EStepMenu.JoinRoom;
                await _userRepository.Update(user);
                _telegramBotService.SendMessage(user.ChatId, "Enter Key Room");
            }

            else
            {
                var menu = new List<string> { "Create Room", "Join Room" };
                _telegramBotService.SendMessage(user.ChatId, "Menu", _telegramBotService.GetKeyboard(menu));
            }

        }
        
        else if (user.Step == (int)EStepMenu.AddRoom)
        {
            var room = new Room()
            {
                Name = message,
                Key = Guid.NewGuid().ToString("N").Substring(0, 10),
                Status = RoomStatus.Created
            };
            await _roomRepository.AddRoomAsync(room);

            user.RoomId = room.Id;
            user.IsAdmin = true;
            user.Step = (int)EStepMenu.InRoom;
            _userRepository.Update(user);

            var roomMenu = new List<string> { "Add products", "Show my products", "Show all users products" };
            _telegramBotService.SendMessage(user.ChatId, "Room added. \n Select menu ", _telegramBotService.GetKeyboard(roomMenu));
        }

        // Step Join Room
        else if (user.Step == (int)EStepMenu.JoinRoom)
        {
            var room = await _roomRepository.GetRoomByKey(message);

            if (room is null)
            {
                _telegramBotService.SendMessage(user.ChatId, "Room not Found");
            }
           
            else if (room is not null)
            {
                user.RoomId = room.Id;
                user.Step = (int)EStepMenu.InRoom;
                await _userRepository.Update(user);

                _telegramBotService.SendMessage(user.ChatId, $"Welcome to {room.Name}");

                var roomMenu = new List<string> { "Add products", "Show my products", "Show all users products","Room status" };
                _telegramBotService.SendMessage(user.ChatId, "Choose menu", _telegramBotService.GetKeyboard(roomMenu));
            }
        }

        // Step In Room
        else if (user.Step == (int)EStepMenu.InRoom)
        {
            
            if (message == "Add products")
            {
                user.Step = (int)EStepMenu.AddProducts;
                await _userRepository.Update(user);

                _telegramBotService.SendMessage(user.ChatId, "Xarajatni shu musolda kiriting 5000-Olma");
            }
            
            else if (message == "Show my products")
            {

                string productList = "";
                int cost = 0;
                


                var userProducts = user.Outlays;

                foreach (var outlay in userProducts!)
                {
                    cost += outlay.Cost;
                    productList += $"{outlay.Cost} - {outlay.Description}\n";
                }

                _telegramBotService.SendMessage(user.ChatId, $"{productList}\nTotal outlays {cost} som");
            }
            
            else if (message == "Room Info")
            {
                var room = await _context.Rooms.Include(r => r.Users)
                    .Include(r => r.Outlays)
                    .FirstOrDefaultAsync(r => r.Id == user.RoomId);
                var roomInfo = $"Room: {room.Name}\n" +
                    $"Users: {room.Users.Count}\n" +
                    $"Total: {room.Outlays.Sum(o => o.Cost)}\n" +
                    $"PerUser: {room.Outlays.Sum(o => o.Cost) / room.Users.Count}";
                _telegramBotService.SendMessage(user.ChatId, roomInfo);
            }
            else
            {
                var roomMenu = new List<string> { "Add products", "Show my products", "Show all users products", "Room Info" };
                _telegramBotService.SendMessage(user.ChatId, "Choose menu", _telegramBotService.GetKeyboard(roomMenu));
            }
        }

        else if (user.Step == (int)EStepMenu.AddProducts)
        {
            if (string.IsNullOrEmpty(message))
            {
                _telegramBotService.SendMessage(user.ChatId, "Xarajatni shu musolda kiriting 5000-Olma");
            }

            else
            {
                var outlaysArray = message.Split("-").ToArray();
                int.TryParse(outlaysArray[0], out var cost);

                if (outlaysArray.Length != 2 || cost == 0)
                {
                    _telegramBotService.SendMessage(user.ChatId, "Xarajatni shu musolda kiriting 5000-Olma");
                }

                else
                {
                    var outlay = new Outlay
                    {
                        Cost = cost,
                        Description = outlaysArray[1],
                        UserId = user.Id,
                        RoomId = user.RoomId,

                    };
                    await _outlayRepositroy.AddOutlayAsync(outlay);

                    user.Step = (int)EStepMenu.InRoom;
                    await _userRepository.Update(user);


                    var roomMenu = new List<string> { "Add products", "Show my products", "Show all users products", "Room info" };
                    _telegramBotService.SendMessage(user.ChatId, "Product Added", _telegramBotService.GetKeyboard(roomMenu));
                }
            }
        }

    }

    private Tuple<long, string, string> GetUpdate(Update update)
    {
        var chatId = update.Message.Chat.Id;
        var message = update.Message.Text;
        var name = update.Message.From.Username ?? update.Message.From.FirstName;

        return new(chatId, message, name);
    }

    public async Task<User> FilterUser(long chatId, string username)
    {
        var user = await _userRepository.GetUserByChatId(chatId);
        if (user is null)
        {
            user = new User
            {
                ChatId = chatId,
                Name = username,
            };

            await _userRepository.AddUserAsync(user);
        }
        return user;
    }
}
