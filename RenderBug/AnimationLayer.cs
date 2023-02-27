using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System.ComponentModel;
using Avalonia.Controls.Shapes;

namespace RenderBug;

public class AnimationLayer : ContentControl
{
    [Category("Animation")]
    public static readonly StyledProperty<Brush> MouseOnBrushProperty =
        AvaloniaProperty.Register<AnimationLayer, Brush>(
            nameof(MouseOnBrush), new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)));

    public static readonly StyledProperty<bool> MouseEnterAnimationProperty =
        AvaloniaProperty.Register<AnimationLayer, bool>(
            nameof(MouseEnterAnimation),
            true
        );

    public static readonly StyledProperty<bool> MouseDownWaveProperty =
        AvaloniaProperty.Register<AnimationLayer, bool>(
            nameof(MouseDownWave),
            true
        );

    protected Rectangle? ActiveRect;
    protected Canvas? PART_RippleCanvasRoot;

    [Category("Animation")]
    public bool MouseDownWave
    {
        get => GetValue(MouseDownWaveProperty);
        set => SetValue(MouseDownWaveProperty, value);
    }

    public Brush MouseOnBrush
    {
        get => GetValue(MouseOnBrushProperty);
        set => SetValue(MouseOnBrushProperty, value);
    }

    public bool MouseEnterAnimation
    {
        get => GetValue(MouseEnterAnimationProperty);
        set => SetValue(MouseEnterAnimationProperty, value);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        BeginMouseEnterAnim(e);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        BeginMouseLeaveAnim(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Find canvas host
        ActiveRect = e.NameScope.Find<Rectangle>(nameof(ActiveRect));
        PART_RippleCanvasRoot = e.NameScope.Find<Canvas>(nameof(PART_RippleCanvasRoot));
    }

    protected override void OnMeasureInvalidated()
    {
        base.OnMeasureInvalidated();

        var radius = CornerRadius.GetAverageCornerRadius();
        PART_RippleCanvasRoot.Clip =
            GeometryHelper.CreateDefiningGeometry(Bounds.Size, radiusX: radius, radiusY: radius);
    }

    protected virtual void BeginMouseEnterAnim(PointerEventArgs? e)
    {
        if (ActiveRect != null && MouseEnterAnimation)
            ActiveRect.Opacity = 1;
    }

    protected virtual void BeginMouseLeaveAnim(PointerEventArgs? e)
    {
        if (ActiveRect != null && MouseEnterAnimation)
            ActiveRect.Opacity = 0;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == MouseEnterAnimationProperty)
        {
            var layer = (AnimationLayer)change.Sender;
            if (layer.Tag == null) return;

            if (!layer.MouseEnterAnimation)
                layer.BeginMouseEnterAnim(null);
            else
                layer.BeginMouseLeaveAnim(null);
        }

        base.OnPropertyChanged(change);
    }
}