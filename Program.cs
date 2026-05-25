using System.Windows.Forms;

namespace ExceptionsDemo
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var form = new Form
            {
                Text = "Exceptions Demo",
                Width = 400,
                Height = 220,
                StartPosition = FormStartPosition.CenterScreen
            };

            var button = new Button
            {
                Text = "Process file",
                Left = 130,
                Top = 70,
                Width = 140
            };

            button.Click += (_, _) =>
            {
                try
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "numbers.txt");
                    var result = ProcessFile(path);

                    MessageBox.Show(form, $"Resultat: {result}", "Klar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(form, $"Filen hittades inte: {ex.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(form, $"Formatfel: {ex.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (DivideByZeroException ex)
                {
                    MessageBox.Show(form, $"Kan inte dividera med noll: {ex.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(form, $"Okänt fel: {ex.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    MessageBox.Show(form, "Cleanup: Logging avslutat anrop.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            form.Controls.Add(button);
            Application.Run(form);

            // Exempel på metod som själv kastar ett undantag (throw)
            static double ProcessFile(string fileName)
            {
                // Om filnamnet är tomt: logiskt fel vi vill signalera
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException("Filnamn får inte vara tomt eller null.", nameof(fileName));
                }

                StreamReader? reader = null;
                try
                {
                    reader = new StreamReader(fileName);

                    string? line = reader.ReadLine();
                    if (line == null)
                        throw new InvalidOperationException("Filen är tom.");

                    // Försöker omvandla text till tal
                    int number = int.Parse(line); // Kan ge FormatException

                    // Division: kan ge DivideByZeroException
                    return 100.0 / number;
                }
                catch (FormatException ex)
                {
                    // Vi kan logga eller omformulera felet
                    Console.WriteLine($"Formatfel i ProcessFile: {ex.Message}");
                    // Vi kan välja att låta metoden "kasta upp" felet
                    throw; // När du i `catch` bara vill logga/analysera,
                           // men låta anroparen (t.ex. en högre nivå i applikationen)
                           // bestämma hur man ska återhämta sig. 
                }
                catch (Exception ex)
                {
                    // Om vi vill ge en mer meningsfull feltyp till anroparen
                    throw new InvalidOperationException(
                    "Det gick inte att processa filen.",
                    ex); // InnerException = ursprunglig fel
                }
                finally
                {
                    // Garanterad stängning av resurs
                    reader?.Close();
                    Console.WriteLine("finally i ProcessFile: StreamReader stängd.");
                }
            }
        }
    }
}

