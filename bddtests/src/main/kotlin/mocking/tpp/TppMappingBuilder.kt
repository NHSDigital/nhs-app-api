package mocking.tpp

import mocking.MappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.session.TppSessionBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import worker.models.demographics.TppUserSession
import mocking.tpp.models.ListRepeatMedication
import mocking.tpp.prescriptions.TppPrescriptionsBuilder
import models.Patient


open class TppMappingBuilder(private val method: String, relativePath: String) : MappingBuilder(method, "$relativePath") {

    internal val HEADER_TYPE = "type"
    internal val HEADER_SUID = "suid"

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)
    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)
    fun listRepeatMedication(patient: Patient) = TppPrescriptionsBuilder(patient)
}