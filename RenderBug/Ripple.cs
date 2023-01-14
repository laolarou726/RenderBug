using Avalonia.Animation.Easings;
using Avalonia.Controls.Shapes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia;
using System;

namespace RenderBug;

public sealed class Ripple : Ellipse
{
    public static readonly TimeSpan Duration = new(0, 0, 0, 0, 500);
    private readonly double _endX;
    private readonly double _endY;

    private readonly double _maxDiam;

    public Ripple(double outerWidth, double outerHeight)
    {
        Width = 0;
        Height = 0;

        _maxDiam = Math.Sqrt(Math.Pow(outerWidth, 2) + Math.Pow(outerHeight, 2));
        _endY = _maxDiam - outerHeight;
        _endX = _maxDiam - outerWidth;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;
        Opacity = 1;
    }

    public static Easing Easing { get; set; } = new CubicEaseOut();

    public void SetupInitialValues(PointerPressedEventArgs e, Control parent)
    {
        var pointer = e.GetPosition(parent);
        Margin = new Thickness(pointer.X, pointer.Y, 0, 0);
    }

    public void RunFirstStep()
    {
        Width = _maxDiam;
        Height = _maxDiam;
        Margin = new Thickness(-_endX / 2, -_endY / 2, 0, 0);
    }

    public void RunSecondStep()
    {
        Opacity = 0;
    }
}