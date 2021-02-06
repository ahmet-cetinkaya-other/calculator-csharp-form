using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        public static Color _lightGray = Color.FromArgb(31, 31, 31);
        public static Color _darkGray = Color.FromArgb(19, 19, 19);
        public static Color _darkBlack = Color.FromArgb(6, 6, 6);
        public static int _clientWidth = 337;
        public static int _controlWidth = 80;
        private DarkLabelBox _lblCalculatorHistory = new DarkLabelBox("lblCalculatorHistory", String.Empty, 0, 0, 60);
        private DarkLabelBox _lblCalculatorResult = new DarkLabelBox("lblCalculatorResult", "0", 0, 60, 100);
        private int _operation;
        private double _result;

        public Form1()
        {
            InitializeComponent();
            using (var stream = File.OpenRead("Calculator.ico"))
            {
                this.Icon = new Icon(stream);
            }
            this.Width = _clientWidth;
            this.Height = _clientWidth * 2;
            this.BackColor = _lightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Calculator";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controls.Add(_lblCalculatorHistory);
            this.Controls.Add(_lblCalculatorResult);
            DarkButton[,] buttons = new DarkButton[6, 4];
            int top = 160;
            int left = 0;
            int number = 9;
            string[] signs = new string[14] { "%", "CE", "C", "←", "1/x", "x²", "√x", "÷", "×", "−", "+", "+/-", ",", "=" };
            int lastSignIndex = 0;
            int lastNumber = 7;
            for (int i = 0; i <= buttons.GetUpperBound(0); i++)
            {
                if (i > 2 && i < 5) lastNumber -= 6;
                else if (i == 5) lastNumber = 0;
                for (int j = 0; j <= buttons.GetUpperBound(1); j++)
                {
                    string buttonText;
                    if ((i == 0 || i == 1) || (i != 5 && j != 0 && j % 3 == 0) || (i == 5 && j != 1)) buttonText = signs[lastSignIndex++];
                    else
                    {
                        buttonText = lastNumber.ToString();
                        lastNumber++;
                    }
                    buttons[i, j] = new DarkButton($"btn{buttonText}", buttonText, left, top, handleClickEvent);
                    left += _controlWidth;
                    this.Controls.Add(buttons[i, j]);
                }
                top += _controlWidth;
                left = 0;
            }
        }

        private void handleClickEvent(object sender, EventArgs e)
        {
            string buttonText = (sender as DarkButton).Text;
            if ((char.IsDigit(buttonText[0]) && buttonText.Length == 1) || (buttonText == "," && !_lblCalculatorResult.Text.Contains(',')))
            {
                if (_lblCalculatorResult.Text.StartsWith("0")) _lblCalculatorResult.Text = String.Empty;
                _lblCalculatorResult.Text += buttonText;
            }
            else
            {
                switch (buttonText)
                {
                    case "%":
                        operation(1);
                        break;

                    case "CE":
                        _lblCalculatorResult.Text = "0";
                        break;

                    case "C":
                        _operation = 0;
                        _lblCalculatorHistory.Text = String.Empty;
                        _lblCalculatorResult.Text = "0";
                        break;

                    case "←":
                        if (_lblCalculatorResult.Text.Length == 1)
                        {
                            _lblCalculatorResult.Text = "0";
                            break;
                        }
                        _lblCalculatorResult.Text =
                            _lblCalculatorResult.Text.Substring(0, _lblCalculatorResult.Text.Length - 1);
                        break;

                    case "1/x":
                        quickOperation(2);
                        break;

                    case "x²":
                        quickOperation(3);
                        break;

                    case "√x":
                        quickOperation(4);
                        break;

                    case "÷":
                        operation(5);
                        break;

                    case "×":
                        operation(6);
                        break;

                    case "-":
                        operation(7);
                        break;

                    case "+":
                        operation(8);
                        break;

                    case "+/-":
                        quickOperation(9);
                        break;

                    case "=":
                        if (_operation > 0) operation(_operation);
                        break;
                }
            }
        }

        private void quickOperation(int operationNumber)
        {
            _lblCalculatorResult.Text = calc(operationNumber).ToString();
        }

        private void operation(int operationNumber)
        {
            if (_operation > 0)
            {
                _result = calc(_operation);
                _lblCalculatorHistory.Text = String.Empty;
                _lblCalculatorResult.Text = _result.ToString();
                _operation = 0;
                return;
            }
            _operation = operationNumber;
            _lblCalculatorHistory.Text += _lblCalculatorResult.Text;
            _lblCalculatorResult.Text = "0";
        }

        private double calc(int operation)
        {
            switch (operation)
            {
                case 1:
                    return double.Parse(_lblCalculatorHistory.Text) % double.Parse(_lblCalculatorResult.Text);

                case 2:
                    return 1 / double.Parse(_lblCalculatorResult.Text);

                case 3:
                    return Math.Pow(double.Parse(_lblCalculatorResult.Text), 2);

                case 4:
                    return Math.Sqrt(double.Parse(_lblCalculatorResult.Text));

                case 5:
                    return double.Parse(_lblCalculatorHistory.Text) / double.Parse(_lblCalculatorResult.Text);

                case 6:
                    return double.Parse(_lblCalculatorHistory.Text) * double.Parse(_lblCalculatorResult.Text);

                case 7:
                    return double.Parse(_lblCalculatorHistory.Text) - double.Parse(_lblCalculatorResult.Text);

                case 8:
                    return double.Parse(_lblCalculatorHistory.Text) + double.Parse(_lblCalculatorResult.Text);

                case 9:
                    return double.Parse(_lblCalculatorResult.Text) * -1;

                default:
                    return 0.0;
            }
        }

        internal class DarkLabelBox : Label
        {
            public DarkLabelBox(string name, string text, int positionX, int positionY, int height)
            {
                Name = name;
                BackColor = _lightGray;
                ForeColor = Color.White;
                Location = new Point(positionX, positionY);
                Text = text;
                TextAlign = ContentAlignment.MiddleRight;
                Font = new Font("Arial", height / 4, FontStyle.Bold);
                AutoSize = false;
                Height = height;
                Width = _clientWidth;
                Padding = new Padding(0, 0, 20, 0);
            }
        }

        internal class DarkButton : Button
        {
            private bool _isDigit;

            public DarkButton(string name, string text, int positionX, int positionY, EventHandler clickEvent)
            {
                Name = name;
                _isDigit = Char.IsDigit(text[0]) && text.Length == 1;
                BackColor = _isDigit ? _darkBlack : _darkGray;
                ForeColor = Color.White;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 1;
                FlatAppearance.BorderColor = _lightGray;
                Width = _controlWidth;
                Height = _controlWidth;
                Text = text;
                Font = new Font("Arial", 12, _isDigit ? FontStyle.Bold : FontStyle.Regular);
                Location = new Point(positionX, positionY);
                Click += clickEvent;
                MouseEnter += new EventHandler(mouseEnter);
                MouseLeave += new EventHandler(mouseLeave);
            }

            private void mouseEnter(object sender, EventArgs e)
            {
                BackColor = Color.FromArgb(52, 52, 52);
            }

            private void mouseLeave(object sender, EventArgs e)
            {
                BackColor = _isDigit ? _darkBlack : _darkGray;
            }
        }
    }
}