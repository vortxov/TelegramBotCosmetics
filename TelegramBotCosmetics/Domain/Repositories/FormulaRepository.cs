using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain.Repositories
{
    public class FormulaRepository
    {
        AppDbContext app; //Переменная для содержания класс с бд

        public FormulaRepository(AppDbContext app) //При создание класс требуется класс с бд
        {
            this.app = app; //В переменную кидается класс с бд
        }


        public async Task DeleteFormula(Guid id) //Функция удаления запроса клиента(поиск запроса идет по айди)
        {
            app.Remove(GetFormulaById(id)); //GetRequestClientById(id) ищет запрос по айди и передает функции Remove строку в бд для удаления
            await app.SaveChangesAsync();//Сохраняем изменения в дб, если не сохранить то не удалится 
        }

        public async Task<Formula> GetFormulaById(Guid id)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Formulas.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Formula> GetFormulaByName(string name)//Функция получения одной строки в бд(поиск запроса идет по айди)
        {
            return app.Formulas.FirstOrDefault(c => c.Name == name);
        }

        public async Task<IQueryable<Formula>> GetFormulas() //Функция для получения всех строк запроса
        {
            return app.Formulas;
        }

        public async Task SaveFormula(Formula entity) //Функция для сохранение строки в таблицу
        {
            if (app.Formulas.FirstOrDefault(x => x.Id == entity.Id) == null) //Проверка есть ли такая строка уже, если возращает пустоту(то есть null) то сохраняем
            {
                await app.Formulas.AddAsync(entity); //Добавляем строку в таблицу
                await app.SaveChangesAsync(); //сохраняем изменения бд
            }
        }

        public async Task UpdateFormula(Formula entity) //Функция обновление строки, то изменяем данные строки, если нет строки то добавляет ее
        {
            app.Formulas.Update(entity); //Апдейт строки
            await app.SaveChangesAsync(); //Сохранение изменений
        }
    }
}
