using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Algorytmika.Windows
{
    /// <summary>
    /// Interaction logic for RouteConfigurationWindow.xaml
    /// </summary>
    public partial class RouteConfigurationWindow : Window
    {
        double Distance { get; set; }
        int City { get; set; }
        internal RouteConfigurationWindow(Algorithm alg)
        {
            InitializeComponent();
            List<int> cities = new List<int>();
            foreach(var c in alg.NodesList)
            {
                cities.Add(c.Position);
            }
            citiesComboBox.ItemsSource = cities;
        }

        public double GetMaxRouteDistance()
        {
            return Convert.ToDouble(DistanceL.Text.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
