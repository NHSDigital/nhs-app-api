using System;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountsConfigResult
    {
        public abstract T Accept<T>(ILinkedAccountsConfigResultVisitor<T> visitor);

        public class Success : LinkedAccountsConfigResult
        {
            public Guid PatientId { get; }

            public SessionConfigurationSettings SessionSettings { get; }

            public LinkedAccountsBreakdownSummary LinkedAccountsBreakdownSummary { get; }

            public Success(
                Guid patientId,
                SessionConfigurationSettings sessionSettings,
                LinkedAccountsBreakdownSummary linkedAccountsBreakdownSummary)
            {
                PatientId = patientId;
                SessionSettings = sessionSettings;
                LinkedAccountsBreakdownSummary = linkedAccountsBreakdownSummary;
            }

            public override T Accept<T>(ILinkedAccountsConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}