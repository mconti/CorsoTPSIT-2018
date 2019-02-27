using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasagnaWPF
{
    public class Lasagna
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Peso { get; set; }
        public double Prezzo { get; set; }
        public Lasagna(){}

        public Lasagna (string riga)
        {
            string[] colonne = riga.Split(';');

            //double peso;
            //double.TryParse(colonne[1], out double peso);
            //double.TryParse(colonne[2], out double prezzo);

            Nome = colonne[0];
            //Peso = peso;
            //Prezzo = prezzo;
        }

    }

    public class Lasagne : List<Lasagna>
    {
        public Lasagne() { }
        public Lasagne( string nomeFile )
        {
            StreamReader fin = new StreamReader(nomeFile);
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

    public class DatabaseLasagne : DbContext
    {
        public DbSet<Lasagna> Lasagne { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LasagneDb.sqlite");
        }
    }
}
