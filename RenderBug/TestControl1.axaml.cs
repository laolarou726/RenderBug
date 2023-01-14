using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace RenderBug
{
    public partial class TestControl1 : UserControl
    {
        public TestControl1()
        {
            InitializeComponent();

            DetailText.Text = string.Join(' ', Enumerable.Repeat(Guid.NewGuid(), 5).Select(g => g.ToString("N")));
        }

        private void InputElement_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            ToDetailButton.Margin = new Thickness(10, 0);
        }

        private void InputElement_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            ToDetailButton.Margin = new Thickness(10, 0, -40, 0);
        }
    }
}
