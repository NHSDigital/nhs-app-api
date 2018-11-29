package mocking.tpp

import mocking.tpp.linkage.TppLinkageGETBuilder
import mocking.tpp.linkage.TppLinkagePOSTBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.models.LinkAccount
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.registration.LinkAccountBuilder
import mocking.tpp.session.TppLogOffBuilder
import mocking.tpp.session.TppSessionBuilder
import models.Patient
import worker.models.demographics.TppUserSession

class TppMappingBuilderAuthentication{

    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)

    fun logOffRequest()= TppLogOffBuilder()

    fun linkAccountRequest(patient: Patient) = LinkAccountBuilder(LinkAccount.forPatient(patient))

    fun linkageKeyPOSTRequest(linkAccount: LinkAccount) = TppLinkagePOSTBuilder(linkAccount)

    fun linkageKeyGetRequest(linkAccount: LinkAccount)= TppLinkageGETBuilder (linkAccount)

    //This is also in my records but for where it is needed it would add confusion if we didn't add
    //the call in here too.
    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)
}

