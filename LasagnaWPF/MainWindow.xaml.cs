using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using System.Xml.Linq;

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
        DatabaseLasagne db = new DatabaseLasagne();

        public MainWindow()
        {
            InitializeComponent();

            using (DatabaseLasagne dbContext = new DatabaseLasagne())
            {
                dbContext.Database.Migrate();
            }

            dgDati.ItemsSource = db.Lasagne.ToList();

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
                    //Peso = peso,
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

        private async void BtnWebApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string response = await client.GetStringAsync(
                                 "https://flr.azurewebsites.net/api/lasagna");

                    var result = JsonConvert.DeserializeObject
                                 <IEnumerable<Lasagna>>(response);

                    dgDati.ItemsSource = result;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ocio!! {err.Message}");
            }
        }

        private void BtnInserisciRecordDb_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(edtPrezzo.Text, out double prezzo);
            Lasagna l = new Lasagna { Nome=edtNome.Text, Peso=edtPeso.Text, Prezzo=prezzo  };

            db.Lasagne.Add(l);
            db.SaveChanges();

            dgDati.ItemsSource = null;
            dgDati.ItemsSource = db.Lasagne.ToList();
        }

        private void BtnEliminaRecordDb_Click(object sender, RoutedEventArgs e)
        {
            Lasagna l = dgDati.SelectedItem as Lasagna;
            if (l != null)
            {
                db.Lasagne.Remove(l);
                db.SaveChanges();

                dgDati.ItemsSource = null;
                dgDati.ItemsSource = db.Lasagne.ToList();
            }

        }
        private async void BtnGetXML_Click(object sender, RoutedEventArgs e)
        {
            // Scarica XML dalla rete
            HttpClient clientXML = new HttpClient();
            var streamXML = await clientXML.GetStreamAsync(@"http://www.piattoforte.it/feed-rss/rss-ricette-giunti.html");

            // Lo trasforma in oggetti Lasagna
            var tutte = (from Elemento in XElement.Load(streamXML).Element("channel").Elements("item")
                        select new Lasagna {
                            Nome = Elemento.Element("title").Value,
                            UrlImmagine = Elemento.Element("description").Value
                        }).ToList();

            // Normalizza gli elementi
            foreach( var l in tutte )
            {
                int inizio, fine;
                inizio = l.UrlImmagine.IndexOf('"') + 1;
                fine = l.UrlImmagine.IndexOf('"', inizio);

                string s = l.UrlImmagine.Substring(inizio, fine-inizio);
                l.UrlImmagine = s;
            }

            // POSTa gli oggetti sulla WEBApi
            try
            {
                using (var client = new HttpClient())
                {
                    // baseAddress è l'URL della WebAPI
                    client.BaseAddress = new Uri("https://flr.azurewebsites.net");

                    foreach (var lasagna in tutte)
                    {
                        // Per usare PostAsJsonAsync, è necessario includere il pacchetto NuGet Microsoft.AspNet.WebApi.Client
                        // HTTP POST è il verbo per aggiungere un record (è la 'C' del CRUD)
                        HttpResponseMessage response = await client.PostAsJsonAsync("/api/lasagna", lasagna);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ocio!! {err.Message}");
            }

        }
    }
}
