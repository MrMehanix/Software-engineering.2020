using SantaCRM.ViewModels;
using System.Windows;

namespace SantaCRM.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPersonWindow.xaml
    /// </summary>
    public partial class AddPersonWindow : Window
    {
        public AddPersonWindow(string id)
        {
            InitializeComponent();
            this.DataContext = new AddPersonWindowViewModel(id);
        }
    }
}
