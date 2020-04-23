using System;
using System.Collections.Generic;
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

            public IEnumerable<LinkedAccount> LinkedAccounts { get; }

            public Success(
                Guid patientId,
                SessionConfigurationSettings sessionSettings,
                IEnumerable<LinkedAccount> linkedAccounts)
            {
                PatientId = patientId;
                SessionSettings = sessionSettings;
                LinkedAccounts = linkedAccounts;
            }

            public override T Accept<T>(ILinkedAccountsConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}