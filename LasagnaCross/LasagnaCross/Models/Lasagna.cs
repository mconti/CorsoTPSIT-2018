using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LasagnaModel
{
    public class Lasagna
    {
        public string Nome { get; set; }
        public double Peso { get; set; }
        public double Prezzo { get; set; }
        public Lasagna() { }

        public Lasagna(string riga)
        {
            string[] colonne = riga.Split(';');

            //double peso;
            double.TryParse(colonne[1], out double peso);
            double.TryParse(colonne[2], out double prezzo);

            Nome = colonne[0];
            Peso = peso;
            Prezzo = prezzo;
        }

    }

    public class Lasagne : List<Lasagna>
    {
        // realFile = "C:\Users\maurizio\AppData\Local\Packages\8f020c4f-7665-4b52-ac39-f87bf3bbe660_s97xw37f0a6wp\LocalCache\Data\in.csv"
        public Lasagne() { }
        public Lasagne(string nomeFile)
        {
            var realFile = Path.Combine(FileSystem.CacheDirectory, nomeFile );
            if (realFile == null || !File.Exists(realFile))
            {
                using (var fout = File.CreateText(realFile))
                {
                    fout.WriteLineAsync("Nome;Prezzo;Peso");
                    fout.WriteLineAsync("Lasagne alla Bolognese;1,2;100");
                }
            }
        

            StreamReader fin = new StreamReader(realFile);
            int quanteColonne = fin.ReadLine().Split(';').Length;
            if (quanteColonne == 3)
            {
                while (!fin.EndOfStream)
                {
                    string riga = fin.ReadLine();
                    Add(new Lasagna(riga));
                }
            }
            else
                throw new Exception("No No !!! Non si fa!!!");
        }
    }
}
