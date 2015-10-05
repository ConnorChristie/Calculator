using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Calculator
{
    public sealed partial class MainPage : Page
    {
        private EquationHandler equationHandler;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(EquationHandler))
            {
                equationHandler = e.Parameter as EquationHandler;

                equationHandler.equationText = this.Equation;

                equationHandler.CursorUpdate();

                CheckAngleType();
            }
            else
            {
                equationHandler = new EquationHandler(this.Equation);

                DispatcherTimer timer = new DispatcherTimer();

                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Tick += new EventHandler<object>(Timer_Tick);
                timer.Start();
            }

            this.Frame.KeyDown += Equation_KeyDown;
            this.Frame.KeyUp += Equation_KeyUp;
        }

        private bool isShiftDown = false;

        private void Equation_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "NumberPad0":
                case "Number0":
                    AddToEquation("0");

                    break;
                case "NumberPad1":
                case "Number1":
                    AddToEquation("1");

                    break;
                case "NumberPad2":
                case "Number2":
                    AddToEquation("2");

                    break;
                case "NumberPad3":
                case "Number3":
                    AddToEquation("3");

                    break;
                case "NumberPad4":
                case "Number4":
                    AddToEquation("4");

                    break;
                case "NumberPad5":
                case "Number5":
                    AddToEquation("5");

                    break;
                case "NumberPad6":
                case "Number6":
                    AddToEquation("6");

                    break;
                case "NumberPad7":
                case "Number7":
                    AddToEquation("7");

                    break;
                case "NumberPad8":
                case "Number8":
                    AddToEquation("8");

                    break;
                case "NumberPad9":
                case "Number9":
                    AddToEquation("9");

                    break;
                case "189":
                case "Subtract":
                    AddToEquation("-");

                    break;
                case "187":
                    if (isShiftDown)
                    {
                        AddToEquation("+");
                    }

                    break;
                case "Add":
                    AddToEquation("+");

                    break;
                case "191":
                case "Divide":
                    AddToEquation("/");

                    break;
                case "190":
                case "Decimal":
                    AddToEquation(".");

                    break;
                case "Back":
                    RemoveFromEquation(1);

                    break;
                case "Shift":
                    isShiftDown = true;

                    break;
                case "Enter":
                    e.Handled = true;

                    break;
                default:
                    AddToEquation(e.Key.ToString());

                    break;
            }
        }

        private void Equation_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                isShiftDown = false;
            } else if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            equationHandler.CursorTick();
        }

        private void AddToEquation(String equation)
        {
            equationHandler.AddElement(EquationElement.ElementType.NORMAL, equation);
        }

        private void RemoveFromEquation(int backspaceLength)
        {
            equationHandler.RemoveLastElement();

            /*
            if (displayEquation.Length - backspaceLength >= 0)
            {
                displayEquation = displayEquation.Substring(0, displayEquation.Length - backspaceLength);

                SetEquation(displayEquation);
            }
             */
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (equationHandler.GetElementsString().Contains("X"))
            {
                Frame.Navigate(typeof(Graph), equationHandler.ToEquation());
            }
            else
            {
                Equation equation = equationHandler.ToEquation();

                Answer.Text = "" + equation.SolveEquation();
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            RemoveFromEquation(1);
        }

        private void Clear_All_Click(object sender, RoutedEventArgs e)
        {
            equationHandler.ClearAll();
        }

        private void Number_1_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("1");
        }

        private void Number_2_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("2");
        }

        private void Number_3_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("3");
        }

        private void Number_4_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("4");
        }

        private void Number_5_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("5");
        }

        private void Number_6_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("6");
        }

        private void Number_7_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("7");
        }

        private void Number_8_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("8");
        }

        private void Number_9_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("9");
        }

        private void Number_0_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("0");
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation(".");
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("+");
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("-");
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("*");
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("÷");
        }

        private void Modulus_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("%");
        }

        private void Variable_X_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("X");
        }

        private void Symbol_LeftParenth_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("(");
        }

        private void Symbol_RightParenth_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation(")");
        }

        private void Symbol_Parenth_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("π");
        }

        private void Left_Arrow_Click(object sender, RoutedEventArgs e)
        {
            equationHandler.MoveCursor(-1);
        }

        private void Right_Arrow_Click(object sender, RoutedEventArgs e)
        {
            equationHandler.MoveCursor(1);
        }

        private void Symbol_Sin_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("Sin(");
        }

        private void Symbol_Cos_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("Cos(");
        }

        private void Symbol_Tan_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("Tan(");
        }

        private void Switch_Angle_Click(object sender, RoutedEventArgs e)
        {
            equationHandler.isDegrees = !equationHandler.isDegrees;

            CheckAngleType();
        }

        private void CheckAngleType()
        {
            if (equationHandler.isDegrees)
            {
                Angle_Type.Text = "Deg";

                Switch_Angle.Content = "Rad";
            }
            else
            {
                Angle_Type.Text = "Rad";

                Switch_Angle.Content = "Deg";
            }
        }

        private void Switch_2nd_Click(object sender, RoutedEventArgs e)
        {
            equationHandler.isSecond = !equationHandler.isSecond;

            Switch_2nd.Background = new SolidColorBrush();

            if (equationHandler.isSecond)
            {
                ((SolidColorBrush)Switch_2nd.Background).Color = Color.FromArgb(255, 56, 56, 56);
            }
            else
            {
                ((SolidColorBrush)Switch_2nd.Background).Color = Color.FromArgb(255, 116, 116, 116);
            }
        }

        private void Symbol_Sqrt_Click(object sender, RoutedEventArgs e)
        {
            AddToEquation("√(");
        }
    }
}
