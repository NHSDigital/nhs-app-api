using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.MarkupExtensions.Responsive
{
    [ContentProperty(nameof(FontSizeOption))]
    public class NhsFontSize : IMarkupExtension<double>
    {
        /// <summary>
        /// Look up the relevant multiplication factor to apply to the base 'NamedSize.Caption' value of 12
        /// to achieve the desired font size.
        /// </summary>
        private static readonly Dictionary<FontSizeOption, double> FontSizeFactorLookup =
            new Dictionary<FontSizeOption, double>
            {
                { FontSizeOption.Small, 1.167 },        // 14
                { FontSizeOption.Medium, 1.417 },       // 17
                { FontSizeOption.Large, 1.83 },         // 22
                { FontSizeOption.Heading2, 1.667 },     // 20
                { FontSizeOption.Heading2Wide, 2 },     // 24
                { FontSizeOption.Title, 2.333 },        // 28
                { FontSizeOption.Heading1, 2.667 },     // 32
                { FontSizeOption.Heading1Wide, 4 },     // 48
            };

        public FontSizeOption FontSizeOption { set; get; } = FontSizeOption.Medium;

        public double ProvideValue(IServiceProvider serviceProvider)
        {
            // 'NamedSize.Caption' is used as a base as it is a consistent size between Android and iOS.
            // On Android 'Device.GetNamedSize' always returns a fixed value as accessibility text scaling is done by the device, not in code.
            // On iOS this returns a varying value based on the device accessibility text scaling setting.
            var responsiveCaptionSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label));

            return FontSizeFactorLookup[FontSizeOption] * responsiveCaptionSize;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<double>).ProvideValue(serviceProvider);
        }
    }
}