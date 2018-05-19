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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Algorytmika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Algorithm alg;

        public MainWindow()
        {
            InitializeComponent();
            alg = new Algorithm();
            CanvasBorder.BorderThickness = new Thickness(1);
            //alg.LoadData(@"D:\Studia\Algorytmika\Algorytmika\Algorytmika\test.txt");
         
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            string filePath = "";
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            try
            {
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (openFileDialog1.ShowDialog() == true)
                {
                    filePath = openFileDialog1.FileName;
                    alg.LoadData(filePath);
                    var maxValueX = alg.NodesList.Max(w => w.X) + 2.5;
                    var maxValueY = alg.NodesList.Max(w => w.Y) + 2.5;
                    canvas.Height = maxValueY;
                    canvas.Width = maxValueX;
                    CanvasBorder.Width = maxValueX + 1;
                    CanvasBorder.Height = maxValueY + 1;
                    foreach (var node in alg.NodesList)
                    {
                        var ellipse = new Ellipse() { Width = 2.5, Height = 2.5, Stroke = new SolidColorBrush(Colors.Black) };
                        Canvas.SetLeft(ellipse, node.X);
                        Canvas.SetTop(ellipse, node.Y);
                        canvas.Children.Add(ellipse);
                    }
                    
                    List<Node> route = alg.GreedyRouteConstruction(7600);
                    int a = route.Count;
                    for (int i=0;i<route.Count-1;i++)
                    {
                        Line line = new Line();
                        line.Stroke = Brushes.Red;

                        line.X1 = route.ElementAt(i).X;
                        line.X2 = route.ElementAt(i+1).X;
                        line.Y1 = route.ElementAt(i).Y;
                        line.Y2 = route.ElementAt(i+1).Y;

                        line.StrokeThickness = 2;
                        canvas.Children.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Nie można odczytać pliku z danymi: " + ex.Message);
            }
        }

        private void ExitAppClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
