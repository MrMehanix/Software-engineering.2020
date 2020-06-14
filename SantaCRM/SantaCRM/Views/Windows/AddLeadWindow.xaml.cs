using SantaCRM.ViewModels;
using System.Windows;

namespace SantaCRM.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPersonWindow.xaml
    /// </summary>
    public partial class AddLeadWindow : Window
    {
        public AddLeadWindow(string id)
        {
            InitializeComponent();
            this.DataContext = new AddLeadWindowViewModel(id);
        }
    }
}
