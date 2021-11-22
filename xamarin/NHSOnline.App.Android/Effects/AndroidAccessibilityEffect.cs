using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Views.Accessibility;
using AndroidX.Core.View.Accessibility;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.Effects;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Action = Android.Views.Accessibility.Action;
using View = Android.Views.View;

[assembly: ResolutionGroupName(NhsAppEffects.ResolutionGroupName)]
[assembly: ExportEffect(typeof(NHSOnline.App.Droid.Effects.AndroidAccessibilityEffect), nameof(AccessibilityEffect))]

namespace NHSOnline.App.Droid.Effects
{
    internal sealed class AndroidAccessibilityEffect : PlatformEffect
    {
        [SuppressMessage("ReSharper", "CA2000",
            Justification = "The AccessibilityDelegate is passed through to Android where it will be collected")]
        protected override void OnAttached()
        {
            if ((Control ?? Container) is { } target &&
                AccessibilityEffect.GetControlType(Element) is { } controlType)
            {
                target.SetAccessibilityDelegate(new AccessibilityDelegate(Element, controlType));
                switch (controlType)
                {
                    case AccessibilityEffect.ControlType.Button:
                    case AccessibilityEffect.ControlType.Link:
                        AutomationProperties.SetIsInAccessibleTree(Element, true);
                        target.KeyPress -= ControlOnKeyPress;
                        target.KeyPress += ControlOnKeyPress;
                        target.FocusChange -= ControlFocusChange;
                        target.FocusChange += ControlFocusChange;
                        break;
                    case AccessibilityEffect.ControlType.Spinner:
                        AutomationProperties.SetIsInAccessibleTree(Element, true);
                        break;
                    case AccessibilityEffect.ControlType.Heading1:
                    case AccessibilityEffect.ControlType.Heading2:
                    default:
                        break;
                }

                if (Element is IVisibleControl visibleControl)
                {
                    visibleControl.VisibilityChangeRequested -= AccessibleControlOnVisibilityChangeRequested;
                    visibleControl.VisibilityChangeRequested += AccessibleControlOnVisibilityChangeRequested;
                }
            }
        }

        private void AccessibleControlOnVisibilityChangeRequested(object sender, IVisibleControl.VisibilityChangeEventArgs e)
        {
            if ((Control ?? Container) is { } target)
            {
                target.ImportantForAccessibility = e.IsVisible ? ImportantForAccessibility.Yes : ImportantForAccessibility.No;
                target.Focusable = e.IsVisible;
            }
        }

        protected override void OnDetached()
        {
            if ((Control ?? Container) is { } target &&
                AccessibilityEffect.GetControlType(Element) is { })
            {
                target.KeyPress -= ControlOnKeyPress;
                target.FocusChange -= ControlFocusChange;
            }

            if (Element is IVisibleControl visibleControl)
            {
                visibleControl.VisibilityChangeRequested -= AccessibleControlOnVisibilityChangeRequested;
            }
        }

        private void ControlFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (Element is AccessibleStackLayout accessibilityLayout)
            {
                accessibilityLayout.IsKeyboardFocused = e.HasFocus;
            }
        }

        private void ControlOnKeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.IsEnterKeyReleaseEvent() &&
                Element is IGestureRecognizers gestureRecognizers &&
                HasViewParent(Element, out var view))
            {
                gestureRecognizers.GestureRecognizers
                    .OfType<TapGestureRecognizer>()
                    .FirstOrDefault(x => x.NumberOfTapsRequired == 1)
                    ?.SendTapped(view);
                e.Handled = true;
            }
        }

        private static bool HasViewParent(Element element, [NotNullWhen(true)] out Xamarin.Forms.View? parentView)
        {
            for (var ancestor = element; ancestor.Parent != null; ancestor = ancestor.Parent)
            {
                if (ancestor is Xamarin.Forms.View view)
                {
                    parentView = view;
                    return true;
                }
            }

            parentView = null;
            return false;
        }

        private sealed class AccessibilityDelegate : View.AccessibilityDelegate
        {
            private readonly WeakReference<Element> _element;
            private readonly AccessibilityEffect.ControlType _controlType;

            public AccessibilityDelegate(Element element, AccessibilityEffect.ControlType controlType)
            {
                _element = new WeakReference<Element>(element);
                _controlType = controlType;
            }

            public override bool PerformAccessibilityAction(View? host, Action action, Bundle? args)
            {
                if (action == Action.Click && _element.TryGetTarget(out var element))
                {
                    if (element is Xamarin.Forms.View view)
                    {
                        // There's an issue with Xamarin Forms TapGestures not working with talkback enabled
                        // https://github.com/xamarin/Xamarin.Forms/issues/9991
                        view.GestureRecognizers.OfType<TapGestureRecognizer>()
                            .FirstOrDefault(x => x.NumberOfTapsRequired == 1)
                            ?.SendTapped(view);

                        return true;
                    }
                }

                return base.PerformAccessibilityAction(host, action, args);
            }

            public override void OnInitializeAccessibilityNodeInfo(View? host, AccessibilityNodeInfo? info)
            {
                base.OnInitializeAccessibilityNodeInfo(host, info);
                if (info != null)
                {
                    var isSelected = _element.TryGetTarget(out var element) && AccessibilityEffect.GetSelected(element);
                    SetAccessibilityNodeInfo(AccessibilityNodeInfoCompat.Wrap(info), _controlType, isSelected);
                }
            }

            private static void SetAccessibilityNodeInfo(
                AccessibilityNodeInfoCompat info,
                AccessibilityEffect.ControlType controlType,
                bool selected)
            {
                switch (controlType)
                {
                    case AccessibilityEffect.ControlType.Button:
                        info.RoleDescription = "Button";
                        info.Selected = selected;
                        info.Clickable = true;
                        break;
                    case AccessibilityEffect.ControlType.Link:
                        info.RoleDescription = "Link";
                        info.Clickable = true;
                        break;
                    case AccessibilityEffect.ControlType.Heading1:
                        info.RoleDescription = "Heading 1";
                        break;
                    case AccessibilityEffect.ControlType.Heading2:
                        info.RoleDescription = "Heading 2";
                        break;
                    case AccessibilityEffect.ControlType.Spinner:
                        info.RoleDescription = "Loading";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(controlType),
                            controlType,
                            $"{nameof(OnInitializeAccessibilityNodeInfo)} doesn't cover all types");
                }
            }
        }
    }
}