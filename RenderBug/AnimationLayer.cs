using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Styling;
using System;

namespace RenderBug;

public class AnimationLayer : ContentControl, IStyleable
{
    [Category("Animation")]
    public static readonly StyledProperty<Brush> MouseOnBrushProperty =
        AvaloniaProperty.Register<AnimationLayer, Brush>(
            nameof(MouseOnBrush), new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)));

    public static readonly StyledProperty<bool> MouseEnterAnimationProperty =
        AvaloniaProperty.Register<AnimationLayer, bool>(
            nameof(MouseEnterAnimation),
            true,
            notifying: PropertyChangedCallback
        );

    public static readonly StyledProperty<bool> MouseDownWaveProperty =
        AvaloniaProperty.Register<AnimationLayer, bool>(
            nameof(MouseDownWave),
            true
        );

    protected Rectangle? ActiveRect;
    protected Canvas? PART_RippleCanvasRoot;

    /// <summary>
    ///     水波反馈
    /// </summary>
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

    /// <summary>
    ///     鼠标移入移出动画
    /// </summary>
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

    // 以下操作是为了显示出BackgroundCanvas
    // protected override int VisualChildrenCount => base.VisualChildrenCount + 1;

    private static void PropertyChangedCallback(AvaloniaObject d, bool arg2)
    {
        var layer = (AnimationLayer)d;
        if (layer.Tag == null) return;

        if (!layer.MouseEnterAnimation)
            layer.BeginMouseEnterAnim(null);
        else
            layer.BeginMouseLeaveAnim(null);
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
}