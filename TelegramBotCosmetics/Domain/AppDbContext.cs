using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Domain
{
    public class AppDbContext : DbContext //Подключение библиотеки баз данных
    {
        public DbSet<Catalog> Catalogs { get; set; } //Созданем таблицу в бд
        public DbSet<Formula> Formulas { get; set; }//Созданем таблицу в бд
        public DbSet<Item> Items { get; set; }//Созданем таблицу в бд
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WhiteFormula> WhiteFormulas { get; set; }
        public DbSet<TypeCatalog> TypeCatalogs { get; set; }
        public DbSet<People> Peoples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //Начальные настройки подключения бд
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);//Добавляем в переменную json файл который находится в проект,
                                                                     //функция ищет appsettings.json в проект и принимает данные которые в нем имеются

            var configuration = builder.Build(); //Создаем настройки с файла

            optionsBuilder.UseSqlServer(configuration["Project:ConnectionString"]); //И берем определенную строку Project:ConnectionString в файле и поключаем бд строкой подключения 
        }
    }
}
