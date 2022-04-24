using OpenQA.Selenium;
using System.Collections.ObjectModel;
using TelegramBotCosmetics.Domain.Entity;

namespace TelegramBotCosmetics.Service
{
    public static class ParsePage
    {
        public static List<Item> CatalogParse(ReadOnlyCollection<IWebElement> liList)
        {
            try
            {
                List<Item> items = new List<Item>();



                foreach (var li in liList)
                {
                    Item item = new Item();

                    item.IdItemOnPage = li.GetAttribute("data-id");
                    item.Tag = new Tag() { Name = li.FindElement(By.ClassName("product-item-category-title")).Text };
                    item.Href = li.FindElement(By.TagName("a")).GetAttribute("href");
                    item.Price = li.FindElement(By.ClassName("price")).Text;
                    item.ItemName = li.FindElement(By.ClassName("catalog-brand-name-span")).Text + " " + li.FindElement(By.ClassName("catalog-product-name-span")).Text;
                    item.Brend = li.FindElement(By.ClassName("catalog-brand-name-span")).Text;
                    if (li.FindElements(By.ClassName("out-of-stock-label")).Count != 0)
                        item.Count = 0;
                    else
                        item.Count = 1;

                    items.Add(item);
                }
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }

        public static Item ItemParse(IWebDriver driver, Item item)
        {
            try
            {
                var sect = driver.FindElement(By.ClassName("pdp__info"));
                var headers = sect.FindElements(By.TagName("header"));

                item.Description = sect.FindElement(By.ClassName("product-description__description")).Text;

                var attributelist = sect.FindElement(By.ClassName("product-attributes__list"));

                foreach (var attribute in attributelist.FindElements(By.ClassName("product-attributes__item")))
                {
                    if(attribute.FindElement(By.ClassName("product-attributes__item-key-text")).Text == "тип продукта")
                    {
                        item.Type = attribute.FindElement(By.ClassName("product-attributes__item-value-text")).Text;
                    }
                    else if (attribute.FindElement(By.ClassName("product-attributes__item-key-text")).Text == "для кого")
                    {
                        item.People = attribute.FindElement(By.ClassName("product-attributes__item-value-text")).Text;
                    }
                }

                item.Type = sect.FindElement(By.ClassName("product-attributes__item-value-text")).Text;

                var buttons = sect.FindElements(By.ClassName("info-tabs__tab"));

                foreach (var button in buttons)
                {
                    var span = button.FindElement(By.TagName("span")).Text;
                    if (button.FindElement(By.TagName("span")).Text == "СОСТАВ")
                    {
                        string str = "";
                        button.Click();
                        var infotabs = driver.FindElements(By.ClassName("info-tabs__item"));
                        foreach (var li in infotabs)
                        {
                            if (li.GetAttribute("data-tab-index") == button.GetAttribute("data-tab-index"))
                            {
                                str = li.FindElement(By.TagName("section")).Text;
                            }
                        }
                        if (str != "")
                        {
                            foreach (var formula in str.Split(", "))
                            {
                                item.Formulas.Add(new Formula() { Name = formula });
                            }
                        }
                    }
                    //if (button.FindElement(By.TagName("span")).Text == "БРЕНД")
                    //{
                    //    button.Click();
                    //    var infotabs = driver.FindElements(By.ClassName("info-tabs__item"));
                    //    foreach (var li in infotabs)
                    //    {
                    //        if (li.GetAttribute("data-tab-index") == button.GetAttribute("data-tab-index"))
                    //        {
                    //            item.Brend = li.FindElement(By.ClassName("product-description__text-primary")).Text;
                    //        }
                    //    }
                    //}

                }


                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
