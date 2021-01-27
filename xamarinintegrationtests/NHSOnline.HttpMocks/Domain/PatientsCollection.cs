using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientsCollection : IPatients, IEnumerable<Patient>
    {
        private readonly ConcurrentDictionary<string, Patient> _idLookup = new ConcurrentDictionary<string, Patient>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<int, Patient> _nhsNumberLookup = new ConcurrentDictionary<int, Patient>();

        public IDisposable Add(Patient patient)
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _idLookup.TryAdd(patient.Id, patient);
            if (patient.Login != patient.Id)
            {
                _idLookup.TryAdd(patient.Login, patient);
            }
            _nhsNumberLookup.TryAdd(patient.NhsNumber.IntValue, patient);

            return new RemovePatient(this, patient);
        }

        private void Remove(Patient patient)
        {
            _idLookup.Remove(patient.Id, out _);
            _idLookup.Remove(patient.Login, out _);
            _nhsNumberLookup.Remove(patient.NhsNumber.IntValue, out _);
        }

        public Patient? LookupById(string id)
        {
            if (_idLookup.TryGetValue(id, out var patient))
            {
                return patient;
            }

            return null;
        }

        public Patient? LookupByNhsNumber(string nhsNumber)
        {
            nhsNumber = nhsNumber ?? throw new ArgumentNullException(nameof(nhsNumber));

            var intValue = int.Parse(nhsNumber.Replace(" ", string.Empty, StringComparison.InvariantCulture), CultureInfo.InvariantCulture);
            if (_nhsNumberLookup.TryGetValue(intValue, out var patient))
            {
                return patient;
            }

            return null;
        }

        public IEnumerable<Patient> All() => _idLookup.Values.Distinct();

        public IEnumerator<Patient> GetEnumerator() => _idLookup.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class RemovePatient : IDisposable
        {
            private readonly PatientsCollection _patientsCollection;
            private readonly Patient _patient;

            public RemovePatient(PatientsCollection patientsCollection, Patient patient)
            {
                _patientsCollection = patientsCollection;
                _patient = patient;
            }

            public void Dispose() => _patientsCollection.Remove(_patient);
        }
    }
}