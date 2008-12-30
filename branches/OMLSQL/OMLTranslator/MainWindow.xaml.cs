using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMLTranslator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DomainModel domainModel;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            domainModel = new DomainModel();
            DataContext = domainModel;
#if DEBUG
            developerOnlyToolbar.Visibility = Visibility.Visible;
#endif
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ConfirmContinue())
            {
                e.Cancel = true;
            }
        }


        private void TargetTextChanged(object sender, TextChangedEventArgs e)
        {
            // Provide instant update on the text box changes
            TextBox textBox = (TextBox)e.OriginalSource;
            TranslatableString translatableString = (TranslatableString)textBox.DataContext;
            translatableString.Target = textBox.Text;
        }

        private void TargetLanguageChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            if (domainModel.CurrentLanguage != languageSelectComboBox.SelectedItem)
            {
                if (ConfirmContinue())
                {
                    domainModel.CurrentLanguage = (CultureInfo) languageSelectComboBox.SelectedItem;
                }
                else
                {
                    languageSelectComboBox.SelectedItem = domainModel.CurrentLanguage;
                }
            }
        }

        private void CanSaveExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = domainModel != null && domainModel.IsDirty;
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            domainModel.Save();
        }

        private bool ConfirmContinue()
        {

            if (!domainModel.IsDirty) return true;
            MessageBoxResult msgResult = MessageBox.Show(
                this,
                "If the current translations are not saved, they will be lost.\r\n\r\nSave the current translations before proceeding?",
                "OMLTranslator", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Cancel);
            if (msgResult == MessageBoxResult.Cancel) return false;

            if (msgResult == MessageBoxResult.Yes)
            {
                try
                {
                    domainModel.Save();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.GetBaseException().Message, "OMLTranslator", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        // Should really be commands, feel free to refactor :)

        private void RemovePseudoTranslationClicked(object sender, RoutedEventArgs e)
        {
            if (ConfirmContinue())
            {
                domainModel.RemovePseudoTranslation();
            }
        }

        private void PseudoTranslateUntranslatedClicked(object sender, RoutedEventArgs e)
        {
            if (ConfirmContinue())
            {
                domainModel.PseudoTranslate(false);
            }
        }

        private void PseudoTranslateAllClicked(object sender, RoutedEventArgs e)
        {
            if (ConfirmContinue())
            {
                domainModel.PseudoTranslate(true);
            }
        }


    }
}
