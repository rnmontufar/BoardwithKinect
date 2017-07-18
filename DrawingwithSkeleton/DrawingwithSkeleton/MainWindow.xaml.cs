using Coding4Fun.Kinect.Wpf;
using Coding4Fun.Kinect.Wpf.Controls;
using Microsoft.Kinect;
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

namespace DrawingwithSkeleton
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor miKinect;
        private static double _itemTop;
        private static double _itemLeft;
        private static double _topBoundary;
        private static double _bottomBoundary;
        private static double _leftBoundary;
        private static double _rightBoundary;
        private static Brush penColor = Brushes.Green;
        private static bool isDrawing;
        private static bool drawingLines;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectButton.Click += new RoutedEventHandler(kinectButton_Clicked);
            lineButton.Click += new RoutedEventHandler(lineButton_Clicked);
            eraseButton.Click += new RoutedEventHandler(eraseButton_Clicked);
            pencilButton.Click += new RoutedEventHandler(pencilButton_Clicked);
            clearButton.Click += new RoutedEventHandler(cleartButton_Clicked);
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("Ninguns sesor conectado", "Ejemplo drawing");
                Application.Current.Shutdown();
                return;
            }

            miKinect = KinectSensor.KinectSensors.FirstOrDefault();

            try
            {
                miKinect.SkeletonStream.Enable();
                miKinect.Start();
            }
            catch
            {
                MessageBox.Show("Fallo al inicializar kinect", "Ejemplo drawing");
                Application.Current.Shutdown();
            }

            miKinect.SkeletonFrameReady += MiKinect_SkeletonFrameReady;
        }

        private void colorButton_Clicked(object sender, RoutedEventArgs e)
        {
            penColor = Brushes.Yellow;
        }

        private void cleartButton_Clicked(object sender, RoutedEventArgs e)
        {
            drawingLines = false;
        }

        private void pencilButton_Clicked(object sender, RoutedEventArgs e)
        {
            trail = new Polyline();
            trail.Stroke = penColor;
            trail.StrokeThickness = 20;
            canvas1.Children.Add(trail);
            penColor = Brushes.Black;
            drawingLines = false;
        }

        private void eraseButton_Clicked(object sender, RoutedEventArgs e)
        {
            penColor = Brushes.White;
            drawingLines = false;
        }

        private void lineButton_Clicked(object sender, RoutedEventArgs e)
        {
            trail = new Polyline();
            trail.Stroke = penColor;
            trail.StrokeThickness = 20;
            canvas1.Children.Insert(canvas1.Children.Count - 2, trail);
            drawingLines = true;
        }

        private void kinectButton_Clicked(object sender, RoutedEventArgs e)
        {
            drawingLines = false;
        }

        private static void CheckButton(HoverButton button, Ellipse thumbStick)
        {
            if (IsItemMidpointInContainer(button, thumbStick))
            {
                button.Hovering();
            }
            else
            {
                button.Release();
            }
        }

        public static bool IsItemMidpointInContainer(FrameworkElement container, FrameworkElement target)
        {
            FindValues(container, target);

            if (_itemTop < _topBoundary || _bottomBoundary < _itemTop)
            {
                //Midpoint of target is outside of top or bottom
                return false;
            }

            if (_itemLeft < _leftBoundary || _rightBoundary < _itemLeft)
            {
                //Midpoint of target is outside of left or right
                return false;
            }

            return true;
        }

        private static void FindValues(FrameworkElement container, FrameworkElement target)
        {
            var containerTopLeft = container.PointToScreen(new Point());
            var itemTopLeft = target.PointToScreen(new Point());

            _topBoundary = containerTopLeft.Y;
            _bottomBoundary = _topBoundary + container.ActualHeight;
            _leftBoundary = containerTopLeft.X;
            _rightBoundary = _leftBoundary + container.ActualWidth;

            //use midpoint of item (width or height divided by 2)
            _itemLeft = itemTopLeft.X + (target.ActualWidth / 2);
            _itemTop = itemTopLeft.Y + (target.ActualHeight / 2);
        }

        private void MiKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            string message = "No hay datos de esquelto";
            Skeleton[] esqueletos = null;

            using(SkeletonFrame framesEsqueleto = e.OpenSkeletonFrame())
            {
                if (framesEsqueleto != null)
                {
                    esqueletos = new Skeleton[framesEsqueleto.SkeletonArrayLength];
                    framesEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    
                    Joint JoingManoDerecha = esqueleto.Joints[JointType.HandRight];
                    Joint JoingManoIzquierda = esqueleto.Joints[JointType.HandLeft];
                    Joint joingCabeza = esqueleto.Joints[JointType.Head];
                    Joint joingHombro = esqueleto.Joints[JointType.ShoulderLeft];
                    SkeletonPoint posicionManoDerecha = JoingManoDerecha.Position;
                    SkeletonPoint posicionManoIzquierda = JoingManoIzquierda.Position;
                    SkeletonPoint posicionCabeza = joingCabeza.Position;
                    SkeletonPoint posicionHombro = joingHombro.Position;
                   
                    ColorImagePoint colorPointMD = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(posicionManoDerecha, ColorImageFormat.RgbResolution1280x960Fps12);
                    ColorImagePoint colorPointMI = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(posicionManoIzquierda, ColorImageFormat.RgbResolution1280x960Fps12);
                    ColorImagePoint colorPointCA = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(posicionCabeza, ColorImageFormat.RgbResolution1280x960Fps12);


                    if (posicionManoIzquierda.Y > posicionHombro.Y)
                    {
                        if (!isDrawing)
                        {
                            isDrawing = true;

                            if (!drawingLines)
                            {
                                trail = new Polyline();
                                trail.Stroke = penColor;
                                trail.StrokeThickness = 20;
                                canvas1.Children.Insert(canvas1.Children.Count - 2, trail);
                            }
                        }
                        message = string.Format("MD depth:{0:0.0} ", "Click");
                        DrawPoint(JoingManoDerecha, penColor);
                        MovePoint(righEllipse, colorPointMD);
                    } else
                    {
                        if (isDrawing)
                        {
                            isDrawing = false;
                        }
                        MovePoint(righEllipse, colorPointMD);
                    }
                    CheckButton(kinectButton, righEllipse);
                    CheckButton(lineButton, righEllipse);
                    CheckButton(eraseButton, righEllipse);
                    CheckButton(pencilButton, righEllipse);
                    CheckButton(clearButton, righEllipse);
                    
                    scalePosition(righEllipse, JoingManoDerecha);


                }
            }

            //textBlockStatus.Text = message;
        }

        private void DrawPoint(Joint joint, Brush color)
        {
            // Create an ellipse.
            //Ellipse ellipse = new Ellipse
            //{
            //    Width = 30,
            //    Height = 40,
            //    Fill = color
            //};

            //// Position the ellipse according to the point's coordinates.
            //Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            //Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            //// Add the ellipse to the canvas.
            //canvas1.Children.Add(ellipse);
            //scalePosition(ellipse, joint);
            Joint scaledJoint = joint.ScaleTo(1280, 720, .3f, .3f);
            trail.Points.Add(new Point { X = scaledJoint.Position.X, Y = scaledJoint.Position.Y });
        }

        private void MovePoint(FrameworkElement element, ColorImagePoint point)
        {
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);
        }

        public void scalePosition(FrameworkElement element, Joint joint)
        {
            //Joint scaledJoint = joint.ScaleTo(1280, 720);
            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joint.ScaleTo(1280, 720, .3f, .3f);
            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);
        }


    }
}
