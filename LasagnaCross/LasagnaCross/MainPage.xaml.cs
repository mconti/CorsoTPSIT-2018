using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LasagnaCross
{
    
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnApri_Clicked(object sender, EventArgs e)
        {
            try
            {
                lvDati1.ItemsSource = new LasagnaModel.Lasagne(@"Data\in.csv");
                lvDati2.ItemsSource = new LasagnaModel.Lasagne(@"Data\in.csv");
            }
            catch (Exception erore)
            {
                DisplayAlert("Errore", erore.Message, "OK");
            }
        }
    }
}
