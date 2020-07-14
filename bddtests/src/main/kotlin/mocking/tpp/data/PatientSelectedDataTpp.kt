package mocking.tpp.data

import mocking.tpp.models.PatientSelectedReply
import mocking.tpp.models.Person
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP

class PatientSelectedDataTpp {
    var selectedPatient = PatientSelectedReply(person = Person(dateOfBirth = goodPatientTPP.age.dateOfBirth))
}
