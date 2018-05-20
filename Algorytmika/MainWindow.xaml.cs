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
        private Route route;

        public MainWindow()
        {
            InitializeComponent();
            alg = new Algorithm();
            route = new Route();
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
                    DrawPoints();

                    route = alg.GreedyRouteConstruction(7600);
                    DrawRoute(route.CalculatedRoute);

                    //show info about route in UI
                    profitL.Content = route.RouteProfit.ToString();
                    pointsL.Content = route.CalculatedRoute.Count.ToString();
                    lengthL.Content = route.Distance.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Nie można odczytać pliku z danymi: " + ex.Message,"Wczytywanie danych",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void DrawPoints()
        {
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
        }

        private void DrawRoute(List<Node> routeView)
        {
            for (int i = 0; i < routeView.Count - 1; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Red;

                line.X1 = routeView.ElementAt(i).X;
                line.X2 = routeView.ElementAt(i + 1).X;
                line.Y1 = routeView.ElementAt(i).Y;
                line.Y2 = routeView.ElementAt(i + 1).Y;

                line.StrokeThickness = 2;
                canvas.Children.Add(line);
            }
            Line line1 = new Line();
            line1.Stroke = Brushes.Red;

            line1.X1 = routeView.ElementAt(0).X;
            line1.X2 = routeView.ElementAt(routeView.Count - 1).X;
            line1.Y1 = routeView.ElementAt(0).Y;
            line1.Y2 = routeView.ElementAt(routeView.Count - 1).Y;

            line1.StrokeThickness = 2;
            canvas.Children.Add(line1);
        }

        private void ExitAppClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (route.CalculatedRoute != null)
            {
                Route o = alg.TwoOpt(route);
                List<Node> optimalized =new List<Node>(o.CalculatedRoute);
                double profit = 0;
                double distance = 0;
                foreach (var n in optimalized)
                {
                    profit = profit + n.Profit;
                }

              

                profitL.Content = profit.ToString();
                lengthL.Content = o.Distance.ToString();
                pointsL.Content = optimalized.Count.ToString();
                canvas.Children.Clear();
                DrawPoints();
                DrawRoute(optimalized);
            }

        }
    }
}
