using Algorytmika.Windows;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

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
        string filePath = "";
        private bool what = false;  //if true then draw points on canvas if false show on map
        ArrayList[] incidenceList= new ArrayList[0];

        public MainWindow()
        {
            InitializeComponent();
            SetMenuDisabled();
            alg = new Algorithm();
            route = new Route();
            CanvasBorder.BorderThickness = new Thickness(1);
            ProgressBar.Visibility = Visibility.Collapsed;
            //alg.LoadData(@"D:\Studia\Algorytmika\Algorytmika\Algorytmika\test.txt");

        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            //clear labels when new file loaded
            profitL.Content = "0";
            lengthL.Content = "0";
            pointsL.Content = "0";
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
                ProgressBar.Visibility = Visibility.Visible;
                LoadDataThread();
            }
        }

        public void LoadDataThread()
        {
            ThreadStart start = LoadDataFromFile;
            start += () =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate ()
                    {
                        ProgressBar.Visibility = Visibility.Collapsed;
                        citiesL.Content = alg.numberOfNodes;
                        pathsL.Content = alg.numberOfPaths;
                        SetMenuEneabled();
                        if(what)
                        {
                            canvas.Children.Clear();
                            scroll.Visibility = Visibility.Visible;
                            bingMap.Visibility = Visibility.Collapsed;
                            DrawPoints();
                        }
                        else if(!what)
                        {
                            bingMap.Children.Clear();
                            scroll.Visibility = Visibility.Collapsed;
                            bingMap.Visibility = Visibility.Visible;
                            DrawPinsOnBingMap();
                        }
                    });
            };
            Thread GetData = new Thread(start);
            GetData.Start();
        }

        private void LoadDataFromFile()
        {
            what = alg.LoadData(filePath);
            if (what)
            {
                incidenceList = Dijkstry.setIncidenceList(alg.numberOfNodes, alg.NodeDistances, alg.NodesList);
            }
            else if (!what)
            {
                incidenceList = Dijkstry.setIncidenceList(alg.numberOfNodes, alg.NodeDistances, alg.NodesList);
                //Dijkstry a;
                //for (int i = 0; i < alg.numberOfNodes - 1; i++)
                //{
                //    for (int j = 0; j < alg.numberOfNodes; j++)
                //    {
                //        if (alg.NodeDistances[i, j] == 0 && i != j)
                //        {
                //            if (j > 93)
                //            {

                //            }
                //            a = new Dijkstry(incidenceList);
                //            int[] tempPath = a.GetPath(i, j);
                //            if (tempPath != null)
                //            {
                //                double distance = a.getPathDistance();
                //                alg.NodeDistances[i, j] = distance;
                //                alg.NodeDistances[j, i] = distance;
                //            }
                //            else
                //            {
                //                alg.NodeDistances[i, j] = double.MaxValue;
                //                alg.NodeDistances[j, i] = double.MaxValue;
                //            }
                //        }
                //    }
                //}
            }
            alg.incidenceList = incidenceList;
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

        private void DrawPolygonOnBingMap(List<Node> route, Color color)
        {
            MapPolyline polygon = new MapPolyline();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(color);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            LocationCollection path = new LocationCollection();
            for (int i = 0; i < route.Count - 2; i++)
            {
                if (alg.NodeDistancesDisjkstry[route.ElementAt(i).Position, route.ElementAt(i + 1).Position] == 0)
                {
                    Dijkstry di = new Dijkstry(incidenceList);
                    int[] pathSD = di.GetPath(route.ElementAt(i).Position, route.ElementAt(i + 1).Position);
                    List<Node> dijkstryPath = new List<Node>();
                    if (pathSD != null)
                    {
                        //convert from int[] to List<Node>
                        foreach (var p in pathSD)
                        {
                            foreach (var n in alg.NodesList)
                            {
                                if (n.Position == p)
                                {
                                    dijkstryPath.Add(n);
                                    break;
                                }
                            }
                        }

                        foreach (var p in dijkstryPath)
                        {
                            path.Add(new Location(p.Y, p.X));
                        }
                    }
                }
                else
                {
                    path.Add(new Location(route.ElementAt(i).Y, route.ElementAt(i).X));
                }
            }
            foreach (var node in route)
            {

                path.Add(new Location(node.Y, node.X));
            }
            path.Add(path.ElementAt(0)); //i do punktu poczatkowego
            polygon.Locations = path;
            bingMap.Children.Add(polygon);
        }

        private void DrawRouteOnBingMap(List<Node> route, Color color,bool Dijkstry)
        {
            MapPolyline polygon = new MapPolyline();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(color);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            LocationCollection path = new LocationCollection();
            foreach (var node in route)
            {
                path.Add(new Location(node.Y, node.X));
            }
            if (!Dijkstry)
            {
                path.Add(path.ElementAt(0));
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

        //zachlanna
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            RouteConfigurationWindow rc = new RouteConfigurationWindow(alg);
            rc.ShowDialog();
            if(rc.DialogResult==true)
            {
                double dist = rc.GetMaxRouteDistance();
                alg.Dmax = dist;
                route = alg.GreedyRouteConstruction();
                if (what == false)
                {
                    DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                }
                else if (what == true)
                {
                    canvas.Children.Clear();
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
                profitL.Content = route.RouteProfit.ToString();
                pointsL.Content = route.CalculatedRoute.Count.ToString();
                lengthL.Content = route.Distance.ToString();
            }
        }
        //zachlanno losowa
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RouteConfigurationWindow rc = new RouteConfigurationWindow(alg);
            rc.ShowDialog();
            if (rc.DialogResult == true)
            {
                double dist = rc.GetMaxRouteDistance();
                alg.Dmax = dist;
                canvas.Children.Clear();
                route = alg.GreedyRandomlyRouteConstruction();
                if (what == false)
                {
                    DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
                profitL.Content = route.RouteProfit.ToString();
                pointsL.Content = route.CalculatedRoute.Count.ToString();
                lengthL.Content = route.Distance.ToString();
            }
        }
        // 2-opt INSERT
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                    canvas.Children.Clear();
                    route = alg.Insert(route, alg.Dmax);
                    if (what == false)
                    {
                        bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                        DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                    }
                    else if (what == true)
                    {
                        canvas.Children.Clear();
                        DrawPoints();
                        DrawRoute(route.CalculatedRoute, Brushes.Red);
                    }
                    profitL.Content = route.RouteProfit.ToString();
                    pointsL.Content = route.CalculatedRoute.Count.ToString();
                    lengthL.Content = route.Distance.ToString();
                }
        }
        // 2opt
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
                if (route != null)
                {
                        route = alg.TwoOpt(route);
                        profitL.Content = route.RouteProfit.ToString();
                        lengthL.Content = route.Distance.ToString();
                        pointsL.Content = route.CalculatedRoute.Count();
                        if (what == false)
                        {
                            bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                            DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                        }
                        else if (what == true)
                        {
                            canvas.Children.Clear();
                            DrawPoints();
                            DrawRoute(route.CalculatedRoute, Brushes.Red);
                        }
                }
        }
        // druga trasa
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                Route temp = alg.ConstructAnotherRoute(4400,route);
                profitL2.Content = temp.RouteProfit.ToString();
                lengthL2.Content = temp.Distance.ToString();
                pointsL2.Content = temp.CalculatedRoute.Count();
                if (what == false)
                {
                    DrawPolygonOnBingMap(temp.CalculatedRoute, Colors.Red);
                }
                else if (what == true)
                {
                    DrawPoints();
                    DrawRoute(temp.CalculatedRoute, Brushes.Blue);
                }
                Console.WriteLine("Druga trasa: ");
                foreach (var n in temp.CalculatedRoute)
                {
                    Console.Write(n.Position + 1 + " ");
                }
                Console.Write(temp.CalculatedRoute.ElementAt(0).Position + 1);
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
        //local search
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (route != null)
            {
                    route = alg.LocalSearch(route);
                    profitL.Content = route.RouteProfit.ToString();
                    lengthL.Content = route.Distance.ToString();
                    pointsL.Content = route.CalculatedRoute.Count();
                    if (what == false)
                    {
                        bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                        DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                    }
                    else if (what == true)
                    {
                        canvas.Children.Clear();
                        DrawPoints();
                        DrawRoute(route.CalculatedRoute, Brushes.Red);
                    }
                }
        }
        //GreedyLocalSearch2
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
                RouteConfigurationWindow rc = new RouteConfigurationWindow(alg);
                rc.ShowDialog();
                if (rc.DialogResult == true)
                {
                    double dist = rc.GetMaxRouteDistance();
                    alg.Dmax = dist;
                    route = alg.GreedyLocalSearch2(500);
                    profitL.Content = route.RouteProfit.ToString();
                    lengthL.Content = route.Distance.ToString();
                    pointsL.Content = route.CalculatedRoute.Count();
                    if (what == false)
                    {
                        bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                        DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                    }
                    else if (what == true)
                    {
                        canvas.Children.Clear();
                        DrawPoints();
                        DrawRoute(route.CalculatedRoute, Brushes.Red);
                    }
                    foreach(var n in route.CalculatedRoute)
                    {
                        Console.Write(n.Position+1+ " ");
                    }
                Console.Write(route.CalculatedRoute.ElementAt(0).Position+1);
                }
        }
        //Dijkstry path
        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            int SRC = 0;
            int DST = 0;
            DijkstryWindow dq = new DijkstryWindow(alg);
            dq.ShowDialog();
            if(dq.DialogResult==true)
            {
                SRC = dq.FROM;
                DST = dq.TO;

                Dijkstry di = new Dijkstry(incidenceList);
                List<Node> dijkstryPath = new List<Node>();
                int[] path = di.GetPath(SRC, DST);
                if(path!=null)
                {
                    //convert from int[] to List<Node>
                    foreach (var p in path)
                    {
                        foreach (var n in alg.NodesList)
                        {
                            if (n.Position == p)
                            {
                                dijkstryPath.Add(n);
                                break;
                            }
                        }
                    }

                    if (what == true)
                    {
                        canvas.Children.Clear();
                        DrawPoints();
                        DrawRoute(dijkstryPath, Brushes.Red);
                    }
                    else
                    {
                        DrawRouteOnBingMap(dijkstryPath, Colors.Red, true);
                    }
                }
                else
                {
                    MessageBox.Show("Niespójność danych, trasa nie istnieje", "Błąd danych wejściowych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SetMenuDisabled()
        {
            RouteConstructionTypeMenu.IsEnabled = false;
            RouteOptimalizationManu.IsEnabled = false;
            AnotherRouteMenu.IsEnabled = false;
            RouteABMenu.IsEnabled = false;
        }

        private void SetMenuEneabled()
        {
            RouteConstructionTypeMenu.IsEnabled = true;
            RouteOptimalizationManu.IsEnabled = true;
            AnotherRouteMenu.IsEnabled = true;
            RouteABMenu.IsEnabled = true;
        }

        //GreedyLocalSerch
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            RouteConfigurationWindow rc = new RouteConfigurationWindow(alg);
            rc.ShowDialog();
            if (rc.DialogResult == true)
            {
                double dist = rc.GetMaxRouteDistance();
                alg.Dmax = dist;
                //number of iterations
                route = alg.GreedyLocalSearch(20);
                profitL.Content = route.RouteProfit.ToString();
                lengthL.Content = route.Distance.ToString();
                pointsL.Content = route.CalculatedRoute.Count();
                if (what == false)
                {
                    bingMap.Children.RemoveAt(bingMap.Children.Count - 1);
                    DrawPolygonOnBingMap(route.CalculatedRoute, Colors.Blue);
                }
                else if (what == true)
                {
                    canvas.Children.Clear();
                    DrawPoints();
                    DrawRoute(route.CalculatedRoute, Brushes.Red);
                }
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            if(what)
            {
                canvas.Children.Clear();
                DrawPoints();
            }
            else
            {
                bingMap.Children.Clear();
                DrawPinsOnBingMap();
            }
        }
    }
}
