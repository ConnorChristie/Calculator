using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Calculator
{
    public sealed partial class Graph : Page
    {
        private Equation equation;

        private int width = (int)Window.Current.Bounds.Width;
        private int height = (int)Window.Current.Bounds.Height;

        private double xOffset = 0; //Minus is right on screen
        private double yOffset = 0; // Minus is lower on screen

        private double thickness = 2.0f;
        private double zoom = 70;
        private double sample;

        private Point m_PreviousContactPoint;

        private bool isHolding = false;
        private bool firstPass = true;

        private List<UIElement> graphElements = new List<UIElement>();

        public Graph()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PointerPressed += new PointerEventHandler(T_PointerPressed);
            PointerReleased += new PointerEventHandler(T_PointerReleased);
            PointerMoved += new PointerEventHandler(T_PointerMoved);

            equation = e.Parameter as Equation;

            DrawGraph();
        }

        private void DrawText(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock();

            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            textBlock.FontSize = 20;

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            textBlock.RenderTransform = new RotateTransform();

            ((RotateTransform)textBlock.RenderTransform).Angle = 180;

            this.graphElements.Add(textBlock);
            this.GraphCanvas.Children.Add(textBlock);
        }

        private void T_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(GraphCanvas);
            m_PreviousContactPoint = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed.
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;

            if (pointerDevType == PointerDeviceType.Pen || (pointerDevType == PointerDeviceType.Mouse && pt.Properties.IsLeftButtonPressed))
            {
                isHolding = true;
            }
            else if (pointerDevType == PointerDeviceType.Touch)
            {
                isHolding = true;
            }
        }

        private void T_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isHolding = false;
        }

        private void T_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(GraphCanvas);
            Point currentContactPt = pt.Position;

            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;

            if (isHolding)
            {
                Debug.Text = "Offs: " + xOffset;

                TranslateGraph(m_PreviousContactPoint.X - currentContactPt.X, (m_PreviousContactPoint.Y - currentContactPt.Y) / 2);

                m_PreviousContactPoint = currentContactPt;
            }
        }

        private void TranslateGraph(double xDelta, double yDelta)
        {
            if (xOffset + xDelta >= width / 2)
            {
                xDelta = width / 2 - xOffset;
            }
            else if (xOffset + xDelta <= -width / 2)
            {
                xDelta = -width / 2 - xOffset;
            }

            if (yOffset + yDelta >= height / 2)
            {
                yDelta = height / 2 - yOffset;
            }
            else if (yOffset + yDelta <= -height / 2)
            {
                yDelta = -height / 2 - yOffset;
            }

            if (xDelta != 0 && yDelta != 0)
            {
                GraphCanvas.Children.Clear();

                xOffset += xDelta;
                yOffset += yDelta;

                foreach (UIElement element in graphElements)
                {
                    Canvas.SetLeft(element, Canvas.GetLeft(element) - xDelta);
                    Canvas.SetTop(element, Canvas.GetTop(element) - yDelta);

                    GraphCanvas.Children.Add(element);
                }
            }
        }

        private void DrawGraph()
        {
            Line line;

            for (double xx = 0; xx < width; xx += zoom)
            {
                double x = xx + (width / 2 - xOffset);
                double x2 = (width / 2 - xOffset) - xx;

                line = new Line()
                {
                    X1 = x,
                    Y1 = -height / 2 + yOffset,
                    X2 = x,
                    Y2 = height + height / 2 + yOffset,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Opacity = 0.3
                };

                this.graphElements.Add(line);
                this.GraphCanvas.Children.Add(line);

                line = new Line()
                {
                    X1 = x2,
                    Y1 = -height / 2 + yOffset,
                    X2 = x2,
                    Y2 = height + height / 2 + yOffset,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Opacity = 0.3
                };

                this.graphElements.Add(line);
                this.GraphCanvas.Children.Add(line);

                if (xx / zoom != 0)
                {
                    TextBlock textBlock = new TextBlock();

                    textBlock.Text = "" + (-xx / zoom);
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkTurquoise);
                    textBlock.FontSize = 20;

                    textBlock.Measure(new Size(10, 10));

                    Canvas.SetTop(textBlock, height / 2 + yOffset - textBlock.ActualHeight + 10);
                    Canvas.SetLeft(textBlock, x + textBlock.ActualWidth / 2);

                    textBlock.RenderTransform = new RotateTransform();

                    ((RotateTransform)textBlock.RenderTransform).Angle = 180;

                    this.graphElements.Add(textBlock);
                    this.GraphCanvas.Children.Add(textBlock);

                    textBlock = new TextBlock();

                    textBlock.Text = "" + (xx / zoom);
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkTurquoise);
                    textBlock.FontSize = 20;

                    textBlock.Measure(new Size(10, 10));

                    Canvas.SetTop(textBlock, height / 2 + yOffset - textBlock.ActualHeight + 10);
                    Canvas.SetLeft(textBlock, x2 + textBlock.ActualWidth / 2);

                    textBlock.RenderTransform = new RotateTransform();

                    ((RotateTransform)textBlock.RenderTransform).Angle = 180;

                    this.graphElements.Add(textBlock);
                    this.GraphCanvas.Children.Add(textBlock);
                }
            }

            for (double yy = 0; yy < height; yy += zoom)
            {
                double y = yy + (height / 2 + yOffset);
                double y2 = (height / 2 + yOffset) - yy;

                line = new Line()
                {
                    X1 = -width / 2 - xOffset,
                    Y1 = y,
                    X2 = width + width / 2 - xOffset,
                    Y2 = y,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Opacity = 0.3
                };

                this.graphElements.Add(line);
                this.GraphCanvas.Children.Add(line);

                line = new Line()
                {
                    X1 = -width / 2 - xOffset,
                    Y1 = y2,
                    X2 = width + width / 2 - xOffset,
                    Y2 = y2,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Opacity = 0.3
                };

                this.graphElements.Add(line);
                this.GraphCanvas.Children.Add(line);

                if (yy / zoom != 0)
                {
                    TextBlock textBlock = new TextBlock();

                    textBlock.Text = "" + (yy / zoom);
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkTurquoise);
                    textBlock.FontSize = 20;

                    textBlock.Measure(new Size(10, 10));

                    Canvas.SetTop(textBlock, y + textBlock.ActualHeight / 2);
                    Canvas.SetLeft(textBlock, width / 2 - xOffset + textBlock.ActualWidth + 10);

                    textBlock.RenderTransform = new RotateTransform();

                    ((RotateTransform)textBlock.RenderTransform).Angle = 180;

                    this.graphElements.Add(textBlock);
                    this.GraphCanvas.Children.Add(textBlock);

                    textBlock = new TextBlock();

                    textBlock.Text = "" + (-yy / zoom);
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkTurquoise);
                    textBlock.FontSize = 20;

                    textBlock.Measure(new Size(10, 10));

                    Canvas.SetTop(textBlock, y2 + textBlock.ActualHeight / 2);
                    Canvas.SetLeft(textBlock, width / 2 - xOffset + textBlock.ActualWidth + 10);

                    textBlock.RenderTransform = new RotateTransform();

                    ((RotateTransform)textBlock.RenderTransform).Angle = 180;

                    this.graphElements.Add(textBlock);
                    this.GraphCanvas.Children.Add(textBlock);
                }
            }

            line = new Line()
            {
                X1 = -width / 2 - xOffset,
                Y1 = height / 2 + yOffset,
                X2 = width + width / 2 - xOffset,
                Y2 = height / 2 + yOffset,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(Colors.DarkTurquoise)
            };

            this.graphElements.Add(line);
            this.GraphCanvas.Children.Add(line);

            line = new Line()
            {
                X1 = width / 2 - xOffset,
                Y1 = -height / 2 + yOffset,
                X2 = width / 2 - xOffset,
                Y2 = height + height / 2 + yOffset,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(Colors.DarkTurquoise)
            };

            this.graphElements.Add(line);
            this.GraphCanvas.Children.Add(line);

            sample = zoom / width;

            double lastX = 0;
            double lastY = 0;

            Debug.Text = "";

            //Draw equation line
            for (double xi = -20; xi <= 20; xi += 0.01)
            {
                double x = xi;
                double y = 0;

                String yString = equation.SolveEquationDouble(-x);

                //Debug.Text += "X: " + x + ", Y: " + yString + "\n";

                if (!double.TryParse(yString, out y))
                {
                    Debug.Text = yString;

                    return;
                }

                x *= zoom;
                y *= zoom;

                x += width / 2 - xOffset;
                y += height / 2 + yOffset;

                if (firstPass)
                {
                    lastX = x;
                    lastY = y;

                    firstPass = false;
                }

                line = new Line()
                {
                    X1 = lastX,
                    Y1 = lastY,
                    X2 = x,
                    Y2 = y,
                    StrokeThickness = thickness,
                    Stroke = new SolidColorBrush(Colors.LimeGreen)
                };

                this.graphElements.Add(line);
                this.GraphCanvas.Children.Add(line);

                lastX = x;
                lastY = y;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), equation.equationHandler);
        }
    }
}
