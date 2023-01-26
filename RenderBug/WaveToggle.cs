using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System;
using Avalonia.Media.Immutable;

namespace RenderBug;

public sealed class WaveToggle : AnimationLayer, IStyleable
{
    public static readonly StyledProperty<ISolidColorBrush?> ActiveBrushProperty = AvaloniaProperty.Register<WaveToggle, ISolidColorBrush?>(
        nameof(ActiveBrush), new SolidColorBrush(Color.FromArgb(64, 255, 0, 0)));

    public static readonly StyledProperty<bool> ActivatedProperty = AvaloniaProperty.Register<WaveToggle, bool>(
        nameof(Activated), notifying: OnActivatedChanged);

    private CancellationTokenSource? _cts;

    private Ripple? _last;
    private byte _pointers;

    [Category("Animation")]
    public ISolidColorBrush? ActiveBrush
    {
        get => GetValue(ActiveBrushProperty);
        set => SetValue(ActiveBrushProperty, value);
    }

    public bool Activated
    {
        get => GetValue(ActivatedProperty);
        set => SetValue(ActivatedProperty, value);
    }

    Type IStyleable.StyleKey => typeof(AnimationLayer);

    private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
    {
        if (Activated || !MouseDownWave) return;
        if (_pointers != 0) return;
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        Task.Delay(200, _cts.Token).ContinueWith(_ => { Dispatcher.UIThread.Post(RemoveLastRipple); });

        // Only first pointer can arrive a ripple
        _pointers++;
        var r = CreateRipple(e, false);
        _last = r;

        // Attach ripple instance to canvas
        PART_RippleCanvasRoot.Children.Add(r);
        r.RunFirstStep();
    }

    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
    {
        _cts?.Cancel();
        RemoveLastRipple();
    }

    private void PointerCaptureLostHandler(object? sender, PointerCaptureLostEventArgs e)
    {
        RemoveLastRipple();
    }

    private void RemoveLastRipple()
    {
        if (_last == null)
            return;

        _pointers--;

        // This way to handle pointer released is pretty tricky
        // could have more better way to improve
        OnReleaseHandler(_last);
        _last = null;
    }

    private void OnReleaseHandler(Ripple r)
    {
        // Fade out ripple
        r.RunSecondStep();

        void RemoveRippleTask(Task arg1)
        {
            Dispatcher.UIThread.Post(() => { PART_RippleCanvasRoot.Children.Remove(r); });
        }

        // Remove ripple from canvas to finalize ripple instance
        Task.Delay(Ripple.Duration).ContinueWith(RemoveRippleTask);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        PointerReleasedHandler(this, e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        PointerPressedHandler(this, e);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);

        PointerCaptureLostHandler(this, e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        OnActivatedChanged(this, false);
    }

    protected override void BeginMouseEnterAnim(PointerEventArgs? e)
    {
        if (!MouseEnterAnimation) return;
        if (ActiveRect != null && !Activated)
            ActiveRect.Opacity = 1;
    }

    protected override void BeginMouseLeaveAnim(PointerEventArgs? e)
    {
        if (!MouseEnterAnimation) return;
        if (ActiveRect != null && !Activated)
            ActiveRect.Opacity = 0;
    }

    private Ripple CreateRipple(PointerPressedEventArgs e, bool center)
    {
        var w = Bounds.Width;
        var h = Bounds.Height;

        var r = new Ripple(w, h)
        {
            Fill = ActiveBrush
        };

        if (center) r.Margin = new Thickness(w / 2, h / 2, 0, 0);
        else r.SetupInitialValues(e, this);

        return r;
    }

    private static void OnActivatedChanged(IAvaloniaObject arg1, bool arg2)
    {
        if (arg1 is not WaveToggle waveToggle) return;
        if (waveToggle.ActiveRect == null) return;

        if (waveToggle.Activated)
        {
            waveToggle.ActiveRect.ClearValue(Shape.FillProperty);

            var path = waveToggle.ActiveBrush == null
                ? nameof(waveToggle.MouseOnBrush)
                : nameof(waveToggle.ActiveBrush);

            waveToggle.ActiveRect.Bind(Shape.FillProperty, new Binding(path)
            {
                Source = waveToggle
            });
        }

        waveToggle.ActiveRect.Opacity = waveToggle.Activated ? 1 : 0;

        if (!waveToggle.Activated)
        {
            waveToggle.ActiveRect.ClearValue(Shape.FillProperty);
            waveToggle.ActiveRect.Bind(Shape.FillProperty, new Binding(nameof(waveToggle.MouseOnBrush))
            {
                Source = waveToggle
            });
        }
    }
}