using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class PeopleRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public PeopleRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeletePeople(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetPeopleById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<People> GetPeopleById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Peoples.Include(x => x.TypeCatalogs).FirstOrDefault(c => c.Id == id);
        }

        public async Task<People> GetPeopleByDataP(string dataP)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Peoples.Include(x => x.TypeCatalogs).FirstOrDefault(c => c.DataP == dataP);
        }

        public async Task<IQueryable<People>> GetPeoples() //Функция для получения всех строк запроса
        {
            return app.Peoples;
        }

        public async Task SavePeople(People entity) //Функция для сохранение строки в таблицу
        {
            if (app.Peoples.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.Peoples.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdatePeople(People entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.Peoples.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
