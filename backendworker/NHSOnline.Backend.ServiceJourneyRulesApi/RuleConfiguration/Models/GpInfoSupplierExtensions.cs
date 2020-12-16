using System;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal static class GpInfoSupplierExtensions
    {
        public static Supplier? ToSupplier(this GpInfoSupplier? gpInfoSupplier)
            => gpInfoSupplier switch
            {
                GpInfoSupplier.Emis => Supplier.Emis,
                GpInfoSupplier.Tpp => Supplier.Tpp,
                GpInfoSupplier.Vision => Supplier.Vision,
                GpInfoSupplier.Microtest => Supplier.Microtest,
                GpInfoSupplier.Fake => Supplier.Fake,
                null => null,
                _ => throw new InvalidOperationException(@$"Cannot convert GpInfoSupplier ""{gpInfoSupplier}"" to Supplier")
            };
    }
}