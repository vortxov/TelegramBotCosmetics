using TelegramBotCosmetics.Domain.Repositories;

namespace TelegramBotCosmetics.Domain
{
    public class DataManager
    {
        public AppDbContext appDbContext { get; set; } //Класс с бд
        public CatalogRepository catalogRepository { get; set; }
        public FormulaRepository formulaRepository { get; set; }
        public ItemRepository itemRepository { get; set; }
        public TagRepository tagRepository { get; set; }
        public WhiteFormulaRepository whiteFormulaRepository { get; set; }
        public TypeCatalogRepository typeCatalogRepository { get; set; }
        public PeopleRepository peopleRepository { get; set; }

        public DataManager()
        {
            appDbContext = new AppDbContext(); //Создаем класс и сохраняем в переменную
            itemRepository = new ItemRepository(appDbContext);
            catalogRepository = new CatalogRepository(appDbContext);
            formulaRepository = new FormulaRepository(appDbContext);
            tagRepository = new TagRepository(appDbContext);
            whiteFormulaRepository = new WhiteFormulaRepository(appDbContext);
            typeCatalogRepository = new TypeCatalogRepository(appDbContext);
            peopleRepository = new PeopleRepository(appDbContext);
        }
    }
}
