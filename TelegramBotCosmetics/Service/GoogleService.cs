using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Service
{
    public class GoogleService
    {
        IWebDriver driver;
        DataManager dataManager;

        public GoogleService()
        {
            dataManager = new DataManager();

            driver = new ChromeDriver("C:\\Vort");
            driver.Manage().Window.Maximize();
        }

        public async void SearchItem()
        {
            IList<Catalog> catalogs = dataManager.catalogRepository.GetCatalogs().Result.ToList();
            foreach (var catalog in catalogs)
            {
                int p = 1;

                int countCatalog = catalog.Items.Count;
                while (true)
                {
                    try
                    {
                        try
                        {

                            driver.Navigate().GoToUrl(catalog.Href + "?p=" + p.ToString());
                            System.Threading.Thread.Sleep(1000);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }
                        var li = driver.FindElements(By.ClassName("item"));
                        p++;
                        if (li.Count() == 0)
                            break;

                        var listItem = ParsePage.CatalogParse(li);

                        List<Item> newItems = new List<Item>();

                        if (listItem != null)
                        {
                            foreach (var item in listItem)
                            {
                                try
                                {
                                    driver.Navigate().GoToUrl(item.Href);
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }


                                var newItem = ParsePage.ItemParse(driver, item);
                                if (newItem != null)
                                {
                                    newItem.Catalogs.Add(catalog);

                                    var tag = dataManager.tagRepository.GetTagByName(newItem.Tag.Name).Result;
                                    if (tag != null)
                                        newItem.Tag = tag;

                                    var formulas = new List<Formula>();
                                    foreach (var formula in newItem.Formulas)
                                    {
                                        var dbformula = dataManager.formulaRepository.GetFormulaByName(formula.Name).Result;
                                        if (dbformula != null)
                                        {
                                            formulas.Add(dbformula);
                                        }
                                        else
                                            formulas.Add(formula);
                                    }

                                    var dbItem = dataManager.itemRepository.GetItemByIdItemOnPage(newItem.IdItemOnPage).Result;

                                    if (dbItem != null)
                                    {
                                        if (dbItem.Price != newItem.Price || dbItem.Tag.Name != newItem.Tag.Name || dbItem.Count != newItem.Count || dbItem.Brend != newItem.Brend)
                                        {
                                            newItem.Id = dbItem.Id;
                                            await dataManager.itemRepository.UpdateItem(newItem);
                                        }
                                        else if(!dbItem.Catalogs.Contains(catalog))
                                        {
                                            dbItem.Catalogs.Add(catalog);
                                            await dataManager.itemRepository.UpdateItem(dbItem);
                                        }
                                    }
                                    else
                                    {
                                        newItems.Add(newItem);
                                        await dataManager.itemRepository.SaveItem(newItem);
                                    }
                                }
                            }

                            catalog.Items.AddRange(newItems);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
                if (catalog.Items.Count != countCatalog)
                    await dataManager.catalogRepository.UpdateCatalog(catalog);
                driver.Close();
                driver = new ChromeDriver("C:\\Vort");
                driver.Manage().Window.Maximize();
            }
            Console.WriteLine("Обработка закончилась");
            driver.Close();
        }

    }
}
