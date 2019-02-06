using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LasagnaWPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnApri.Click += Pippo;
            btnApri.Click += Pluto;
        }

        private void Pluto(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eccolo");
        }

        private void Pippo(object sender, RoutedEventArgs e)
        {
            List<Lasagna> lasagne = new List<Lasagna>();

            StreamReader fin = new StreamReader("in.csv");
            fin.ReadLine();

            while(!fin.EndOfStream)
            {
                string riga = fin.ReadLine();
                string[] colonne = riga.Split(';');

                //double peso;
                double.TryParse(colonne[1], out double peso);
                double.TryParse(colonne[2], out double prezzo);

                Lasagna l = new Lasagna
                {
                    Nome = colonne[0],
                    Peso = peso,
                    Prezzo = prezzo
                };

                lasagne.Add(l);
            }
            dgDati.ItemsSource = lasagne;

        }

        private void BtnApri2_Click(object sender, RoutedEventArgs e)
        {
            List<Lasagna> lasagne = new List<Lasagna>();

            StreamReader fin = new StreamReader("in.csv");
            fin.ReadLine();

            while (!fin.EndOfStream)
            {
                string riga = fin.ReadLine();
                lasagne.Add(new Lasagna(riga));
            }
            dgDati.ItemsSource = lasagne;
        }

        private void BtnApri3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgDati.ItemsSource = new Lasagne("in.csv");
            }
            catch (Exception erore)
            {
                MessageBox.Show(erore.Message);
            }
        }
    }
}
