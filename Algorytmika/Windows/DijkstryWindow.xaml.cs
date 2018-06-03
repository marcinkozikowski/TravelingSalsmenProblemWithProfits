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
    public partial class DijkstryWindow : Window
    {
        public int FROM { get; set; }
        public int TO { get; set; }
        internal DijkstryWindow(Algorithm alg)
        {
            InitializeComponent();
            List<string> list = new List<string>();
            foreach(var n in alg.NodesList)
            {
                list.Add(n.Position.ToString());
            }
            fromCombo.ItemsSource = list;
            toCombo.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FROM = Convert.ToInt32(fromCombo.SelectedItem);
            TO = Convert.ToInt32(toCombo.SelectedItem);
            this.DialogResult = true;
        }
    }
}
