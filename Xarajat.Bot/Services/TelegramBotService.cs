using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Xarajat.Bot.Options;

namespace AutoTest.Services;

public class TelegramBotService
{
    private readonly TelegramBotClient _bot;

    public TelegramBotService(IOptions<XarajatBotOptions> options)
    {
        _bot = new TelegramBotClient(options.Value.BotToken);
    }



    // Userga message va button yuboradigan funksiya
    public void SendMessage(long chatId, string message, IReplyMarkup reply = null)
    {
        _bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
    }


    // Userga message, image, button yuboradigan funksiya
    public void SendMessage(long chatId, string message, Stream image, IReplyMarkup reply = null)
    {
        _bot.SendPhotoAsync(chatId, new InputOnlineFile(image), message, replyMarkup: reply);
    }


    public void EditMessageButtons(long chatId, int messageId, InlineKeyboardMarkup reply)
    {
        _bot.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: reply);
    }


    // KeyBoardButtons 
    public ReplyKeyboardMarkup GetKeyboard(List<string> buttonsText)
    {
        var buttons = new KeyboardButton[buttonsText.Count][];

        for (var i = 0; i < buttonsText.Count; i++)
        {
            buttons[i] = new KeyboardButton[] { new KeyboardButton(buttonsText[i]) };
        }
        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }


    // InlineButtons   
    public InlineKeyboardMarkup GetInlineKeyboard(List<string> buttonsText, int? correctAnswerIndex = null, int? questionIndex = null) //???
    {
        var buttons = new InlineKeyboardButton[buttonsText.Count][];

        for (var i = 0; i < buttonsText.Count; i++)
        {
            buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text : buttonsText[i],
                callbackData : correctAnswerIndex == null ? buttonsText[i] : $"{correctAnswerIndex}, {i}, {questionIndex}") };
        }

        return new InlineKeyboardMarkup(buttons);
    }

}