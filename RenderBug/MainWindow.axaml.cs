using System.Linq;
using Avalonia.Controls;

namespace RenderBug
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List1.Items = Enumerable.Repeat(1, 10);
        }
    }
}