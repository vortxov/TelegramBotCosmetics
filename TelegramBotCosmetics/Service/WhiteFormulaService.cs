using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotCosmetics.Domain;
using TelegramBotCosmetics.Domain.Entity;
using Word = Microsoft.Office.Interop.Word;

namespace TelegramBotCosmetics.Service
{
    public class WhiteFormulaService
    {
        static DataManager dataManager = new DataManager();

        public async Task UpdateWhiteList()
        {
            Word.Application app = new Word.Application();
            try
            {
                app.Documents.Open(@"C:\Vort\TelegramBotCosmetics\TelegramBotCosmetics\WhiteList.docx");
                Word.Document doc = app.ActiveDocument;
                for (int i = 1; i < doc.Paragraphs.Count; i++)
                {
                    string parText = doc.Paragraphs[i].Range.Text;

                    string formula;

                    WhiteFormula whiteFormula = new WhiteFormula();

                    var sp = parText.Split("V ");
                    if (sp.Length == 1)
                    {
                        sp = sp[0].Split("L ");
                        whiteFormula.V = false;
                    }
                    else
                    {
                        sp = sp[1].Split("L ");
                        whiteFormula.V = true;
                    }

                    if (sp.Length == 1)
                    {
                        whiteFormula.L = false;
                        formula = sp[0].Split("\r")[0];
                    }
                    else
                    {
                        whiteFormula.L = true;
                        formula = sp[1].Split("\r")[0];
                    }

                    var wfDb = dataManager.whiteFormulaRepository.GetWhiteFormulaByName(formula.ToLower().Split("(")[0].Split("/")[0]);
                    if (wfDb == null)
                    {
                         whiteFormula.Name = formula.ToLower().Split("(")[0].Split("/")[0];
                        await dataManager.whiteFormulaRepository.SaveWhiteFormula(whiteFormula);
                    }


                }
                Console.WriteLine("Update successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                app.Quit();
            }
        }
    }
}
