
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotCosmetics.Domain;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Service;

public class Handlers
{
    public static DataManager dataManager = new DataManager();

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) //Функция для отображение ошибки с ботом,
                                                                                                                                //если будет ошибка то бот не закроется
                                                                                                                                //а просто отобразит ошибку 
    {
        var ErrorMessage = exception switch //Проверка типа ошибка, пока всего один тип и все любой тип
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}", //Если ошибка в апи запросе
            _ => exception.ToString() //Если не прошлая ошибка, то выводит сообщение этой ошибки
        };

        Console.WriteLine(ErrorMessage);//Выводит в консоль ошибку
        return Task.CompletedTask; //Возвращает выполненую задачу
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)//Функция для ответа на сообщения клиента
    {
        var handler = update.Type switch  //Проверяем тим сообщения, либо это обычное сообщение либо кнопка или еще чтото
        {
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!), //Если обычное сообщение то вызывает BotOnMessageReceived
        //    //UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!), //Если изменение сообщения
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!), //Кнопка
            UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!), //Обращение к боту
        //    //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!), //А это я сам не ебу вроде изменение обращение к боту
        //    //                                                                                                         //но как работает хз
        //    //_ => UnknownUpdateHandlerAsync(botClient, update)//Если ничего из предыдущих 
        };

        try
        {
            await handler; //Проверяет были бы ошибки при выполнение прошлых функций
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken); //Если были то отображаем ошибку
        }
    }

    private static async Task<Message> BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
    {
        return null;
    }

    private static async Task<Message> BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var strData = callbackQuery.Data.Split(":");

        if(strData[0] == "forwhom")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var peoples = dataManager.peopleRepository.GetPeoples().Result.ToArray();

            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(peoples[0].Name, "type:" + peoples[0].DataP),
                                InlineKeyboardButton.WithCallbackData(peoples[1].Name, "type:" + peoples[1].DataP),
                                InlineKeyboardButton.WithCallbackData(peoples[2].Name, "type:" + peoples[2].DataP)});


            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: "Выберите для кого:",
                                                        replyMarkup: inlineKeyboard);
        }

        if(strData[0] == "type")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var typeCatalogs = dataManager.peopleRepository.GetPeopleByDataP(strData[1]).Result.TypeCatalogs.ToArray();

            for(int i = 0; i < typeCatalogs.Count(); i++)
            {
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(typeCatalogs[i].Name, "nextcatalog:" + strData[1] + ":" + typeCatalogs[i].Name + ":0") });
            }

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: "Выберите категорию:",
                                                        replyMarkup: inlineKeyboard);
        }

        if(strData[0] == "nextcatalog")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var catalogs = dataManager.catalogRepository.GetCatalogs().Result.ToArray();

            List<Catalog> newCatalog = new List<Catalog>();

            foreach (var catalog in catalogs)
            {
                if(strData[1] == "W" && catalog.People == "женский" && catalog.Type.Name == strData[2])
                {
                    newCatalog.Add(catalog);
                }
                else if (strData[1] == "M" && catalog.People == "мужской" && catalog.Type.Name == strData[2])
                {
                    newCatalog.Add(catalog);
                }
                else if (strData[1] == "C" && catalog.People == "для детей" && catalog.Type.Name == strData[2])
                {
                    newCatalog.Add(catalog);
                }
            }

             catalogs = newCatalog.ToArray();

            for (int i = 0 + (int.Parse(strData[3]) * 5); i < 5 * (1 + int.Parse(strData[3])) && i < catalogs.Length; i++)
            {
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(catalogs[i].Name, "peta:" + strData[1] + ":" + " " + ":" + catalogs[i].Id) });
            }
            if (int.Parse(strData[3]) == 0 && catalogs.Length > 5)
            {
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", "nextcatalog:" + strData[1] + ":" + strData[2] + ":1") });
            }
            else if (catalogs.Length - int.Parse(strData[3]) * 5 < 5 && catalogs.Length > 0)
            {
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "nextcatalog:" + strData[1] + ":" + strData[2]
                                                                                                + ":"+ (int.Parse(strData[3]) - 1)) });
            }
            else if (catalogs.Length > 5 && int.Parse(strData[3]) > 1)
            {
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "nextcatalog:" + strData[1] + ":" + strData[2]
                                                                                                + ":" + (int.Parse(strData[3]) - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", "nextcatalog:" + strData[1] + ":" + strData[2]
                                                                                                + ":"+ (int.Parse(strData[3]) + 1))});
            }

            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "forwhom:") });

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: "Выберите каталог:",
                                                        replyMarkup: inlineKeyboard);
        }

       
        if (strData[0] == "peta")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Любая", "price:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + "A"),
                                InlineKeyboardButton.WithCallbackData("Веганская", "price:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + "V"),
                                InlineKeyboardButton.WithCallbackData("Без тестов над животными", "price:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + "L")});
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться", "type:" + strData[1]) });

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: "Какую косметику вы предпочитаете:",
                                                        replyMarkup: inlineKeyboard);
        }

        if(strData[0] == "price")
        {
            string text = "Ценновая категория.\n Выберите до какой ценный искать:";

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("До 500 рублей", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":500:0"),
                                InlineKeyboardButton.WithCallbackData("До 1000 рублей", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":1000:0")});
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("До 1500 рублей", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":1500:0"),
                                InlineKeyboardButton.WithCallbackData("До 2000 рублей", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":2000:0")});
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Без ограничения", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":0:0") });
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться", "peta: " + strData[1] + ":" + strData[2] + ":" + strData[3]) });

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: text,
                                                        replyMarkup: inlineKeyboard);
        }


        if (strData[0] == "item")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var catalog = dataManager.catalogRepository.GetCatalogById(Guid.Parse(strData[3])).Result;

            var items = catalog.Items.ToArray();

            List<Item> newItems = new List<Item>();

            foreach (var item in items)
            {
                if(item.Count != 0 && item.Formulas.Count != 0)
                    newItems.Add(item);
            }

            items = Filter(newItems, strData, dataManager).ToArray();

            string textItems = "";

            if (items.Length > 0)
            {
                textItems += items[int.Parse(strData[6])].ItemName + "\n" +
                             items[int.Parse(strData[6])].Tag.Name + "\n" +
                             "Цена: " + items[int.Parse(strData[6])].Price + "\n\n" +
                             items[int.Parse(strData[6])].Description + "\n\n" +
                             "Состав: ";

                foreach (var formula in items[int.Parse(strData[6])].Formulas)
                {
                    textItems += formula.Name + " ";
                }

                textItems += "\n\n Ссылка товара на сайт: " + items[int.Parse(strData[6])].Href;

                textItems += "\n\n\n\n";


                if (int.Parse(strData[6]) == 0 && items.Length > 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":1") });
                    
                }
                else if (items.Length - int.Parse(strData[6]) <= 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":" + (int.Parse(strData[6]) - 1)) });
                    
                }
                else if (items.Length - int.Parse(strData[6]) > 1 && int.Parse(strData[6]) > 0)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":" + (int.Parse(strData[6]) - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":" + (int.Parse(strData[6]) + 1))});
                   
                }
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Аналог", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" 
                                                                                    + strData[5] + ":" + strData[6] + ":" + items[int.Parse(strData[6])].IdItemOnPage + ":0"),
                                    InlineKeyboardButton.WithCallbackData("Дешевле", "ch:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                    + strData[5] + ":" + strData[6] + ":" + items[int.Parse(strData[6])].IdItemOnPage + ":0")});

                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }
            else
            {
                textItems = "Товаров не найдено по вашему запросу";
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }

           

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: textItems,
                                                        replyMarkup: inlineKeyboard);
        }

        if(strData[0] == "an")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var catalog = dataManager.catalogRepository.GetCatalogById(Guid.Parse(strData[3])).Result;

            var items = catalog.Items.ToArray();

            List<Item> newItems = new List<Item>();

            var itemForAn = dataManager.itemRepository.GetItemByIdItemOnPage(strData[7]).Result;

            foreach (var item in items)
            {
                if (item.Count != 0 && item.Formulas.Count != 0)
                    newItems.Add(item);
            }

            items = newItems.ToArray();

            newItems = new List<Item>();

            foreach (var item in items)
            {
                if(item.Id != itemForAn.Id)
                {
                    if(item.Type == itemForAn.Type && item.People == itemForAn.People)
                    {
                        double priceItem;
                        var priceitem = item.Price.Split(" ₽");
                        priceitem = priceitem[0].Split("от ");
                        if (priceitem.Length > 1)
                        {
                            priceItem = int.Parse(priceitem[1].Replace(" ", ""));
                        }
                        else
                        {
                            priceItem = int.Parse(priceitem[0].Replace(" ", ""));
                        }

                        double priceItemForAn;
                        var priceitemForAn = itemForAn.Price.Split(" ₽");
                        priceitemForAn = priceitemForAn[0].Split("от ");
                        if (priceitemForAn.Length > 1)
                        {
                            priceItemForAn = double.Parse(priceitemForAn[1].Replace(" ", ""));
                        }
                        else
                        {
                            priceItemForAn = double.Parse(priceitemForAn[0].Replace(" ", ""));
                        }

                        double procent = priceItemForAn / 100 * 20;

                        double negativeprice;

                        if(priceItem > priceItemForAn)
                        {
                            negativeprice = priceItem - priceItemForAn;
                        }
                        else
                        {
                            negativeprice = priceItemForAn - priceItem;
                        }
                        if(negativeprice < procent)
                        {
                            newItems.Add(item);
                        }
                    }
                }
            }

            items = newItems.ToArray();

            string textItems = "";

            if (items.Length > 0)
            {
                textItems += items[int.Parse(strData[8])].ItemName + "\n" +
                             items[int.Parse(strData[8])].Tag.Name + "\n" +
                             "Цена: " + items[int.Parse(strData[8])].Price + "\n\n" +
                             items[int.Parse(strData[8])].Description + "\n\n" +
                             "Состав: ";

                foreach (var formula in items[int.Parse(strData[8])].Formulas)
                {
                    textItems += formula.Name + " ";
                }

                textItems += "\n\n Ссылка товара на сайт: " + items[int.Parse(strData[8])].Href;

                textItems += "\n\n\n\n";

                if (int.Parse(strData[8]) == 0 && items.Length > 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" 
                                                                                             + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) + 1)) });
                   
                }
                else if (items.Length - int.Parse(strData[8]) <= 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                           + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) - 1))});
                    
                }
                else if (items.Length - int.Parse(strData[8]) > 1 && int.Parse(strData[8]) > 0)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                           + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) + 1))});
                   
                }
                //buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Аналог", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":" + strData[6] + ":" + items[int.Parse(strData[6])].IdItemOnPage + ":0") });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6]) });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }
            else
            {
                textItems = "Товаров не найдено по вашему запросу";
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6]) });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }

            

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: textItems,
                                                        replyMarkup: inlineKeyboard);
        }

        if (strData[0] == "ch")
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var catalog = dataManager.catalogRepository.GetCatalogById(Guid.Parse(strData[3])).Result;

            var items = catalog.Items.ToArray();

            List<Item> newItems = new List<Item>();

            var itemForAn = dataManager.itemRepository.GetItemByIdItemOnPage(strData[7]).Result;

            foreach (var item in items)
            {
                if (item.Count != 0 && item.Formulas.Count != 0)
                    newItems.Add(item);
            }

            items = newItems.ToArray();

            newItems = new List<Item>();

            foreach (var item in items)
            {
                if (item.Id != itemForAn.Id)
                {
                    if (item.Type == itemForAn.Type && item.People == itemForAn.People)
                    {
                        double priceItem;
                        var priceitem = item.Price.Split(" ₽");
                        priceitem = priceitem[0].Split("от ");
                        if (priceitem.Length > 1)
                        {
                            priceItem = int.Parse(priceitem[1].Replace(" ", ""));
                        }
                        else
                        {
                            priceItem = int.Parse(priceitem[0].Replace(" ", ""));
                        }

                        double priceItemForAn;
                        var priceitemForAn = itemForAn.Price.Split(" ₽");
                        priceitemForAn = priceitemForAn[0].Split("от ");
                        if (priceitemForAn.Length > 1)
                        {
                            priceItemForAn = double.Parse(priceitemForAn[1].Replace(" ", ""));
                        }
                        else
                        {
                            priceItemForAn = double.Parse(priceitemForAn[0].Replace(" ", ""));
                        }

                        double procent = priceItemForAn / 100 * 20;

                        double negativeprice;

                        if (priceItem < priceItemForAn)
                        {
                            negativeprice = priceItemForAn - priceItem;
                            if (negativeprice > procent)
                            {
                                newItems.Add(item);
                            }
                        }
                    }
                }
            }

            items = newItems.ToArray();

            string textItems = "";

            if (items.Length > 0)
            {
                textItems += items[int.Parse(strData[8])].ItemName + "\n" +
                             items[int.Parse(strData[8])].Tag.Name + "\n" +
                             "Цена: " + items[int.Parse(strData[8])].Price + "\n\n" +
                             items[int.Parse(strData[8])].Description + "\n\n" +
                             "Состав: ";

                foreach (var formula in items[int.Parse(strData[8])].Formulas)
                {
                    textItems += formula.Name + " ";
                }

                textItems += "\n\n Ссылка товара на сайт: " + items[int.Parse(strData[8])].Href;

                textItems += "\n\n\n\n";

                if (int.Parse(strData[8]) == 0 && items.Length > 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", "ch:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                             + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) + 1)) });

                }
                else if (items.Length - int.Parse(strData[8]) <= 1)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "ch:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                           + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) - 1))});

                }
                else if (items.Length - int.Parse(strData[8]) > 1 && int.Parse(strData[8]) > 0)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "ch:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                           + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", "ch:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6] + ":" + strData[7] + ":" + (int.Parse(strData[8]) + 1))});

                }
                //buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Аналог", "an:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":" + strData[5] + ":" + strData[6] + ":" + items[int.Parse(strData[6])].IdItemOnPage + ":0") });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6]) });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }
            else
            {
                textItems = "Товаров не найдено по вашему запросу";
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "item:" + strData[1] + ":" + strData[2] + ":" + strData[3] + ":" + strData[4] + ":"
                                                                                       + strData[5] + ":" + strData[6]) });
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Вернуться в начало", "forwhom:") });
            }



            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return await botClient.EditMessageTextAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        messageId: callbackQuery.Message.MessageId,
                                                        text: textItems,
                                                        replyMarkup: inlineKeyboard);
        }



        throw new NotImplementedException();
    }


    private static List<Item> Filter(List<Item> items, string[] data, DataManager dataManager)
    {
        List<Item> result = new List<Item>();
        List<Item> resWhiteBrend = new List<Item>();
        if (data[4] != "A")
        {
            foreach (Item item in items)
            {
                if (item.Brend != null)
                {
                    var formula = dataManager.whiteFormulaRepository.GetWhiteFormulaByName(item.Brend.ToLower().Split("(")[0].Split("/")[0]);
                    if (formula != null)
                    {
                        if (data[4] == "V" && formula.V)
                        {
                            resWhiteBrend.Add(item);
                        }
                        else if (data[4] == "L" && formula.L)
                        {
                            resWhiteBrend.Add(item);
                        }
                    }

                }
            }
        }
        else
            resWhiteBrend = items;
        if(data[5] == "0")
        {
            resWhiteBrend.Sort((x, y) => y.Price.CompareTo(x.Price));
            result = resWhiteBrend;
        }
        else
        {
            foreach (var item in resWhiteBrend)
            {
                int price;
                var priceitem = item.Price.Split(" ₽");
                priceitem = priceitem[0].Split("от ");
                if(priceitem.Length > 1)
                {
                    price = int.Parse(priceitem[1].Replace(" ", ""));
                }
                else
                {
                    price = int.Parse(priceitem[0].Replace(" ", ""));
                }

                if (price <= int.Parse(data[5]))
                {
                    result.Add(item);
                }
            }

            result.Sort((x, y) => y.Price.CompareTo(x.Price));
        }

        return result;
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Receive message type: {message.Type}");
        if (message.Type != MessageType.Text)
            return;

        var action = message.Text!.Split(' ')[0] switch
        {
            "/start" => SendStartInlineKeyboard(botClient, message),
            _ => SendStartInlineKeyboard(botClient, message)
        };
        Message sentMessage = await action;
        Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
    }

    private static async Task<Message> SendStartInlineKeyboard(ITelegramBotClient botClient, Message message)
    {
        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Женщинам", "type:" + "W"),
                                InlineKeyboardButton.WithCallbackData("Детям", "type:" + "C"),
                                InlineKeyboardButton.WithCallbackData("Мужчинам", "type:" + "M")});

        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Выберите для кого:",
                                                    replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendMessange(ITelegramBotClient botClient, Message message)
    {

        

        int result;
        if(int.TryParse(message.Text, out result))
        {

        }



        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Женщинам", "type:" + "W"),
                                InlineKeyboardButton.WithCallbackData("Детям", "type:" + "C"),
                                InlineKeyboardButton.WithCallbackData("Мужчинам", "type:" + "M")});

        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Выберите для кого:",
                                                    replyMarkup: inlineKeyboard);
    }

    private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult) //Функция не нужна пока
    {
        Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
        return Task.CompletedTask;
    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update) //Функция не нужна пока
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}
