using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Android.OS;
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
                AccessibilityEffect.GetAccessibilityControlType(Element) is { } controlType)
            {
                AutomationProperties.SetIsInAccessibleTree(Element, true);
                target.SetAccessibilityDelegate(new AccessibilityDelegate(Element, controlType));
                switch (controlType)
                {
                    case AccessibilityEffect.AccessibilityControlType.Button:
                    case AccessibilityEffect.AccessibilityControlType.Link:
                        target.KeyPress -= ControlOnKeyPress;
                        target.KeyPress += ControlOnKeyPress;
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnDetached()
        {
            if ((Control ?? Container) is { } target &&
                AccessibilityEffect.GetAccessibilityControlType(Element) is { })
            {
                target.KeyPress -= ControlOnKeyPress;
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
            private readonly AccessibilityEffect.AccessibilityControlType _controlType;

            public AccessibilityDelegate(Element element, AccessibilityEffect.AccessibilityControlType controlType)
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
                    AddAccessibilityActions(AccessibilityNodeInfoCompat.Wrap(info), _controlType);
                }
            }

            private static void AddAccessibilityActions(
                AccessibilityNodeInfoCompat info,
                AccessibilityEffect.AccessibilityControlType controlType)
            {
                switch (controlType)
                {
                    case AccessibilityEffect.AccessibilityControlType.Button:
                        info.RoleDescription = "Button";
                        info.Clickable = true;
                        break;
                    case AccessibilityEffect.AccessibilityControlType.Link:
                        info.RoleDescription = "Link";
                        info.Clickable = true;
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