using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace GUIEnabledATM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ATM atm;

        public MainWindow()
        {
            atm = new();
            InitializeComponent();
            Thread th = new(atm.SysDeployment);
            th.Start();

            atm.monitor._port1.BytesSent += Update_Screen_Content;
            atm.monitor._port2.BytesSent += Update_Screen_ChangeNumberDisplay;
            atm.monitor._port3.BytesSent += Update_Screen_WarningDisplay;

            foreach (var key in atm.keypad._port1)
                key.BytesSent += Update_Screen_ClearNumberDisplay;
        }

        private void Update_Screen_ChangeNumberDisplay(object src, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (atm.SCB._numInputCount > 0)
                {
                    int k = atm.monitor._port2.RecvInteger();
                    System.Diagnostics.Debug.WriteLine("Attempting to display PIN: " + k);
                    inputTextBox.Text = k.ToString();
                }
                else
                {
                    inputTextBox.Text = "";
                }
            });
        }

        private void Update_Screen_ClearNumberDisplay(object src, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                inputTextBox.Text = "";
            });
        }

        private void Keypad_Button_Click(object src, RoutedEventArgs e)
        {
            try
            {
                Button _myButton = (Button)src;
                string? _argStr = _myButton.CommandParameter.ToString();
                if (_argStr != null)
                {
                    int portNum = Convert.ToInt32(_argStr);
                    System.Diagnostics.Debug.WriteLine("Send signal to button " + portNum);
                    atm.keypad._port2[portNum].Send((byte)1);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Button click failed");
            }
        }

        private void Update_Screen_Content(object src, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                trueTextBox.Text = atm.monitor._port1.RecvString();
            });
        }

        private void Update_Screen_WarningDisplay(object src, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                errTextBox.Text = atm.monitor._port3.RecvString();
            });
        }

        private void Function_Button_Click(object src, RoutedEventArgs e)
        {
            try
            {
                Button _myButton = (Button)src;
                string? _argStr = _myButton.CommandParameter.ToString();
                if (_argStr != null)
                {
                    int portNum = Convert.ToInt32(_argStr);
                    atm.keypad._port1[portNum-1].Send((byte)1);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Button click failed");
            }
        }

        private void Add_Card_Event(object src, RoutedEventArgs e)
        {
            try
            {
                Button _myButton = (Button)src;
                string? _argStr = _myButton.CommandParameter.ToString();
                if (_argStr != null)
                {
                    System.Diagnostics.Debug.WriteLine("About to scan card!");
                    atm.cardScanner._port.Send(Convert.ToInt32(_argStr));
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Button click failed");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
