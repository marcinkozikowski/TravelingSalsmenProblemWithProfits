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
        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;

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
                    canvas.Children.Clear();
                    filePath = openFileDialog1.FileName;
                    alg.LoadData(filePath);
                    DrawPoints();
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
            //CanvasBorder.Width = maxValueX + 1;
            //CanvasBorder.Height = maxValueY + 1;
            foreach (var node in alg.NodesList)
            {
                var ellipse = new Ellipse() { Width = 4, Height = 4, Stroke = new SolidColorBrush(Colors.Black) };
                ellipse.ToolTip = "Pozycja: "+node.Position +"\nProfit: "+node.Profit+"\nX: "+ node.X + "\nY: " + node.Y;
                Canvas.SetLeft(ellipse, node.X);
                Canvas.SetTop(ellipse, node.Y);
                canvas.Children.Add(ellipse);
            }
        }

        private void DrawRoute(List<Node> routeView,Brush color)
        {
            for (int i = 0; i < routeView.Count - 1; i++)
            {
                Line line = new Line();
                line.Stroke = color;

                line.X1 = routeView.ElementAt(i).X;
                line.X2 = routeView.ElementAt(i + 1).X;
                line.Y1 = routeView.ElementAt(i).Y;
                line.Y2 = routeView.ElementAt(i + 1).Y;

                line.StrokeThickness = 2;
                canvas.Children.Add(line);
            }
            Line line1 = new Line();
            line1.Stroke = color;

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
                canvas.Children.Clear();
                DrawPoints();
                route = alg.GreedyRouteConstruction(7600);
                DrawRoute(route.CalculatedRoute, Brushes.Red);
                profitL.Content = route.RouteProfit.ToString();
                pointsL.Content = route.CalculatedRoute.Count.ToString();
                lengthL.Content = route.Distance.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
                canvas.Children.Clear();
                DrawPoints();
                route = alg.GreedyRandomlyRouteConstruction(7600);
                DrawRoute(route.CalculatedRoute, Brushes.Red);
                //show info about route in UI
                profitL.Content = route.RouteProfit.ToString();
                pointsL.Content = route.CalculatedRoute.Count.ToString();
                lengthL.Content = route.Distance.ToString();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                canvas.Children.Clear();
                DrawPoints();
                Route a = alg.Insert(route, 7600);
                profitL.Content = a.RouteProfit.ToString();
                lengthL.Content = a.Distance.ToString();
                pointsL.Content = a.CalculatedRoute.Count();
                canvas.Children.Clear();
                DrawPoints();
                DrawRoute(a.CalculatedRoute, Brushes.Red);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                canvas.Children.Clear();
                DrawPoints();
                route = alg.TwoOpt(route);
                //List<Node> optimalized = new List<Node>(o.CalculatedRoute);
                //double profit = 0;
                //double distance = 0;
                //foreach (var n in optimalized)
                //{
                //    profit = profit + n.Profit;
                //}
                profitL.Content = route.RouteProfit.ToString();
                lengthL.Content = route.Distance.ToString();
                pointsL.Content = route.CalculatedRoute.Count();
                DrawRoute(route.CalculatedRoute, Brushes.Red);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                Route temp = alg.ConstructAnotherRoute(7600);
                profitL.Content = temp.RouteProfit.ToString();
                lengthL.Content = temp.Distance.ToString();
                pointsL.Content = temp.CalculatedRoute.Count();
                DrawRoute(temp.CalculatedRoute, Brushes.Blue);
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(canvas);

            if (zoom > 1)
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
            }
            else
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            }
        }

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoom = zoomSlider.Value;

            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            if (zoom > 1)
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size from mouse position
            }
            else
            {
                canvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            }
        }
    }
}
