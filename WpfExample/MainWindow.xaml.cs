using MaterialDesignThemes.Wpf;
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
using WpfExample.Dialogs;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Asks for confirmation
            BeforeRemoveItem();
        }

        private string RemoveItem()
        {
            /// In my project I'm calling a DELETE on a Rest Server 
            /// and if an error ocurred, we return the error message...

            // "Simulates" a delay from server..
            System.Threading.Thread.Sleep(3000);

            // Fake result
            return "An error has ocurred in Server";

        }

        private async void BeforeRemoveItem()
        {
            var messageDialog = new DialogQuestion
            {
                Message = {Text = "Do you want remove this item?"}
            };

            var result = await DialogHost.Show(messageDialog, "RootDialog", OnSelectAnswer);

        }

        private void OnSelectAnswer(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false)
                return;

            // Abort current process...
            eventArgs.Cancel();

            // Creates a new dialog, to show the progress bar...
            DialogProgress progresso = new DialogProgress();
            eventArgs.Session.UpdateContent(progresso);

            // Then starts a task..
            var t = Task<string>.Run(() =>
            {
                // Calls a methdo from server...
                return RemoveItem();
            })
            .ContinueWith(resultFromServer =>
            {
                // Now, with the answer from server we can do extra things..

                // If there is no message, we just close the current Message
                if (resultFromServer.Result == string.Empty)
                {
                    eventArgs.Session.Close(false);
                }
                else
                {
                    // If there is an error message, we show a simple dialog (with just an "OK" button) and shows the error..
                    eventArgs.Cancel();
                    eventArgs.Session.UpdateContent(new DialogOk { Message = { Text = resultFromServer.Result } });

                }
            }, TaskScheduler.FromCurrentSynchronizationContext() );
        }

    }
}
