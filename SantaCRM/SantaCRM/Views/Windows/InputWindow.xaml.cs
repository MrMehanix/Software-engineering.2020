using SantaCRM.ViewModels;
using System.Windows;

namespace SantaCRM.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow(int SelectedSetting)
        {
            InitializeComponent();
            this.DataContext = new InputWindowViewModel(SelectedSetting);
        }
    }
}
