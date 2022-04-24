using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class TypeCatalogRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public TypeCatalogRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteTypeCatalog(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetTypeCatalogById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<TypeCatalog> GetTypeCatalogById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.TypeCatalogs.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IQueryable<TypeCatalog>> GetCatalogs() //Функция для получения всех строк запроса
        {
            return app.TypeCatalogs.Include(x => x.Peoples);
        }

        public async Task SaveTypeCatalog(TypeCatalog entity) //Функция для сохранение строки в таблицу
        {
            if (app.TypeCatalogs.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.TypeCatalogs.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateTypeCatalog(TypeCatalog entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.TypeCatalogs.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
