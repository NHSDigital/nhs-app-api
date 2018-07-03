package mocking.tpp

import mocking.MappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.session.TppSessionBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import worker.models.demographics.TppUserSession

open class TppMappingBuilder(private val method: String, relativePath: String) : MappingBuilder(method, "$relativePath") {

    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)
}