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

// Slide per connesione WebApi
// https://docs.google.com/presentation/d/1YOSatpxjg3obuBWjtmst1metB2TMZkrxeHkMieIYpig/edit#slide=id.g4a074c92ce_0_12

// url della WebApi che leggeremo!!!
//https://flr.azurewebsites.net/api/lasagna

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

            // Click è una property di Button di tipo Event
            // l'operatore += agisce su Event aggiungendo reference a metodi da chiamare allo scatenarsi dell'evento (click)
            // Qui, al premere del pulsante quindi, vengono chiamati in sequenza il metodo Pippo e il metodo Pluto
            btnApri.Click += Pippo;
            btnApri.Click += Pluto;
        }

        private void Pluto(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eccolo");
        }

        private void Pippo(object sender, RoutedEventArgs e)
        {
            // Primo sistema: usiamo una List<Lasagna>
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
            // Secondo sistema: usiamo un Lasagne (che, derivando da List<Lasagna>, "è un" List<Lasagna>)
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
            // Terzo sistema(consigliato per aderire al SOC): sfruttiamo il costruttore di Lasagne per spostare al suo interno la logica di apertura file etc...
            // Essendo soggetto a eccezioni, "chiudiamo" la sua istanziazione all'interno di un  try catch
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
