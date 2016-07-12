// Valentin CHAILLOU
// 4A-AL2 ESGI

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

namespace MonPlotterWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PlotterViewModel ViewModel = new PlotterViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void MiseAJourFiltre(object sender, SelectionChangedEventArgs selection)
        {
            if(selection.AddedItems.Count > 0)
                ViewModel.FiltreDonnées(selection.AddedItems.Cast<Capteur>(), true);
            else
                ViewModel.FiltreDonnées(selection.RemovedItems.Cast<Capteur>(), false);
        }

        private void MiseAJourGrandeurs(object sender, SelectionChangedEventArgs e)
        {
            // Pas plus de deux Grandeurs différentes
            if (listBoxGrandeurs.SelectedItems.Count > 2)
                listBoxGrandeurs.SelectedItems.RemoveAt(0);     // Appelle récursivement cette méthode
            else
                listBoxCapteurs.ItemsSource = ViewModel.FiltreGrandeurs(listBoxGrandeurs.SelectedItems.Cast<Grandeur>());
        }

        private void MiseAJourDates(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FiltreDates();
        }

        private void MiseAJourAnalyse(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FiltreAnalyses();
        }
    }
}
