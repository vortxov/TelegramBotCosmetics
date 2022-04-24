using Microsoft.EntityFrameworkCore;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class CatalogRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public CatalogRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteCatalog(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetCatalogById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<Catalog> GetCatalogById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Catalogs.Include(x => x.Items).ThenInclude(t => t.Tag).Include(x => x.Items).ThenInclude(f => f.Formulas).FirstOrDefault(c => c.Id == id);
        }

        public async Task<IQueryable<Catalog>> GetCatalogs() //Функция для получения всех строк запроса
        {
            return app.Catalogs.Include(x => x.Items).Include(x => x.Type);
        }

        public async Task SaveCatalog(Catalog entity) //Функция для сохранение строки в таблицу
        {
            if (app.Catalogs.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.Catalogs.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateCatalog(Catalog entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.Catalogs.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
