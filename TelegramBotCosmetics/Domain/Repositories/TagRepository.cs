using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class TagRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public TagRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteTag(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetTagById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<Tag> GetTagById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Tags.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Tag> GetTagByName(string name)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Tags.FirstOrDefault(c => c.Name == name);
        }

        public async Task<IQueryable<Tag>> GetTags() //Функция для получения всех строк запроса
        {
            return app.Tags;
        }

        public async Task SaveTag(Tag entity) //Функция для сохранение строки в таблицу
        {
            if (app.Tags.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.Tags.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateTag(Tag entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.Tags.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
