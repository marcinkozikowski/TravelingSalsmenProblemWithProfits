using Algorytmika.Windows;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections;
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
        //TODO jakies okienko gdzie bedzie mozna wpisywac parametry trasy tzn ile kilometrow ma miec np mozna nawet gdzies z boku to dorobic
        private Algorithm alg;
        private Route route;
        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;
        private bool what = false;  //if true then draw poin on canvas if false show on map

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

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog1.ShowDialog() == true)
            {
                canvas.Children.Clear();
                filePath = openFileDialog1.FileName;
                bool what = alg.LoadData(filePath);
                if (what)
                {
                    scroll.Visibility = Visibility.Visible;
                    bingMap.Visibility = Visibility.Collapsed;
                    citiesL.Content = alg.numberOfNodes;
                    pathsL.Content = alg.numberOfPaths;
                    DrawPoints();
                }
                else if (!what)
                {
                    scroll.Visibility = Visibility.Collapsed;
                    bingMap.Visibility = Visibility.Visible;
                    ArrayList[] incidenceList;
                    incidenceList = Dijkstry.setIncidenceList(alg.numberOfNodes, alg.NodeDistances, alg.NodesList);
                    Dijkstry a;
                    for (int i = 0; i < alg.numberOfNodes - 1; i++)
                    {
                        for (int j = 0; j < alg.numberOfNodes; j++)
                        {
                            if (alg.NodeDistances[i, j] == 0 && i != j)
                            {
                                if (j > 93)
                                {

                                }
                                a = new Dijkstry(incidenceList);
                                int[] tempPath = a.GetPath(i, j);
                                if (tempPath != null)
                                {
                                    double distance = a.getPathDistance();
                                    alg.NodeDistances[i, j] = distance;
                                    alg.NodeDistances[j, i] = distance;
                                }
                                else
                                {
                                    alg.NodeDistances[i, j] = double.MaxValue;
                                    alg.NodeDistances[j, i] = double.MaxValue;
                                }
                            }
                        }
                    }
                    citiesL.Content = alg.numberOfNodes;
                    pathsL.Content = alg.numberOfPaths;
                    DrawPinsOnBingMap();
                }
            }
            //clear labels when new file loaded
            profitL.Content = "";
            lengthL.Content = "";
            pointsL.Content = "";
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
                ellipse.ToolTip = "Pozycja: " + node.Position + "\nProfit: " + node.Profit + "\nX: " + node.X + "\nY: " + node.Y;
                Canvas.SetLeft(ellipse, node.X);
                Canvas.SetTop(ellipse, node.Y);
                canvas.Children.Add(ellipse);
            }
        }

        private void DrawPinsOnBingMap()
        {
            foreach (var node in alg.NodesList)
            {
                // The pushpin to add to the map.
                Pushpin pin = new Pushpin();
                pin.Location = new Location(node.Y, node.X);
                pin.ToolTip = "Pozycja: " + node.Position + "\nProfit: " + node.Profit + "\nX: " + node.X + "\nY: " + node.Y;
                // Adds the pushpin to the map.
                bingMap.Children.Add(pin);
            }
        }

        private void DrawPolygonOnBingMap(List<Node> route)
        {
            MapPolyline polygon = new MapPolyline();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            LocationCollection path = new LocationCollection();
            foreach (var node in route)
            {
                path.Add(new Location(node.Y, node.X));
            }
            polygon.Locations = path;
            bingMap.Children.Add(polygon);
        }

        private void DrawRoute(List<Node> routeView, Brush color)
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
            if(what==false)
            {
                DrawPolygonOnBingMap(route.CalculatedRoute);
            }
            else if(what==true)
            {
                DrawRoute(route.CalculatedRoute, Brushes.Red);
            }
            profitL.Content = route.RouteProfit.ToString();
            pointsL.Content = route.CalculatedRoute.Count.ToString();
            lengthL.Content = route.Distance.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            DrawPoints();
            route = alg.GreedyRandomlyRouteConstruction(7600);
            if (what == false)
            {
                DrawPolygonOnBingMap(route.CalculatedRoute);
            }
            else if (what == true)
            {
                DrawRoute(route.CalculatedRoute, Brushes.Red);
            }
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
                if (what == false)
                {
                    bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                    DrawPolygonOnBingMap(route.CalculatedRoute);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                canvas.Children.Clear();
                route = alg.TwoOpt(route);

                profitL.Content = route.RouteProfit.ToString();
                lengthL.Content = route.Distance.ToString();
                pointsL.Content = route.CalculatedRoute.Count();
                if (what == false)
                {
                    bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                    DrawPolygonOnBingMap(route.CalculatedRoute);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
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
                if (what == false)
                {
                    bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                    DrawPolygonOnBingMap(route.CalculatedRoute);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
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

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                canvas.Children.Clear();
                route = alg.LocalSearch(route, 7600);

                profitL.Content = route.RouteProfit.ToString();
                lengthL.Content = route.Distance.ToString();
                pointsL.Content = route.CalculatedRoute.Count();
                if (what == false)
                {
                    bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                    DrawPolygonOnBingMap(route.CalculatedRoute);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            //route = alg.GreedyLocalSearch(20);
            route = alg.GreedyLocalSearch2(100);

            profitL.Content = route.RouteProfit.ToString();
            lengthL.Content = route.Distance.ToString();
            pointsL.Content = route.CalculatedRoute.Count();
            if (what == false)
            {
                bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                DrawPolygonOnBingMap(route.CalculatedRoute);
            }
            else if (what == true)
            {
                DrawPoints();
                DrawRoute(route.CalculatedRoute, Brushes.Red);
            }
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            RouteConfigurationWindow a = new RouteConfigurationWindow();
            a.ShowDialog();
        }
    }
}
