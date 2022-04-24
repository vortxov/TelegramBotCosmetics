using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class WhiteFormulaRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public WhiteFormulaRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteWhiteFormula(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetWhiteFormulaById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            await app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<WhiteFormula> GetWhiteFormulaById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.WhiteFormulas.FirstOrDefault(c => c.Id == id);
        }

        public WhiteFormula GetWhiteFormulaByName(string name)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.WhiteFormulas.FirstOrDefault(c => c.Name == name);
        }
         
        public async Task<IQueryable<WhiteFormula>> GetFormulas() //Функция для получения всех строк запроса
        {
            return app.WhiteFormulas;
        }

        public async Task SaveWhiteFormula(WhiteFormula entity) //Функция для сохранение строки в таблицу
        {
            if (app.WhiteFormulas.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.WhiteFormulas.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateWhiteFormula(WhiteFormula entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.WhiteFormulas.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
