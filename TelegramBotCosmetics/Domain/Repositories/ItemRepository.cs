using Microsoft.EntityFrameworkCore;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class ItemRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public ItemRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteItem(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetItemById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<Item> GetItemById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Items.Include(x => x.Tag).Include(f => f.Formulas).FirstOrDefault(c => c.Id == id);
        }

        public async Task<Item> GetItemByIdItemOnPage(string IdItemOnPage)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Items.FirstOrDefault(c => c.IdItemOnPage == IdItemOnPage);
        }

        public async Task<IQueryable<Item>> GetItems() //Функция для получения всех строк запроса
        {
            return app.Items;
        }

        public async Task SaveItem(Item entity) //Функция для сохранение строки в таблицу
        {
            if (app.Items.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.Items.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateItem(Item entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.Items.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
