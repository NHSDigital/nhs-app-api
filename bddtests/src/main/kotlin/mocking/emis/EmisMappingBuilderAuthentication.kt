package mocking.emis

import mocking.emis.demographics.EmisDemographicsBuilder
import mocking.emis.linkage.EmisLinkageGETBuilder
import mocking.emis.linkage.EmisLinkagePOSTBuilder
import mocking.emis.me.EmisMeApplicationsBuilder
import mocking.emis.me.EmisMeBuilder
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddVerificationRequest
import mocking.emis.session.EmisEndUserSessionBuilder
import mocking.emis.session.EmisSessionBuilder
import models.Patient

class EmisMappingBuilderAuthentication(private var configuration: EmisConfiguration?, private val method: String){

    fun meRequest(patient: Patient) = EmisMeBuilder(configuration!!, method, patient)

    fun meApplicationsRequest(patient: Patient, model: LinkApplicationRequestModel) = EmisMeApplicationsBuilder(
            configuration!!, patient.endUserSessionId, model)

    fun endUserSessionRequest() = EmisEndUserSessionBuilder(configuration!!)

    fun sessionRequest(patient: Patient) = EmisSessionBuilder(configuration!!, patient)

    fun linkageKeyGetRequest(request: AddVerificationRequest) = EmisLinkageGETBuilder(request)

    fun linkageKeyPOSTRequest(request: AddNhsUserRequest) = EmisLinkagePOSTBuilder(request)

    //This is also in my records but for where it is needed it would add confusion if we didn't add
    //the call in here too.
    fun demographicsRequest(patient: Patient) = EmisDemographicsBuilder(configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId, patient.sessionId)
}