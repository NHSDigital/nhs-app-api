package mocking.tpp

import mocking.tpp.linkage.TppLinkageGETBuilder
import mocking.tpp.linkage.TppLinkagePOSTBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.models.LinkAccount
import mocking.tpp.registration.LinkAccountBuilder
import mocking.tpp.session.TppLogOffBuilder
import mocking.tpp.session.TppSessionBuilder
import models.Patient

class TppMappingBuilderAuthentication{

    fun authenticateRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)

    fun logOffRequest()= TppLogOffBuilder()

    fun linkAccountRequest(patient: Patient) = LinkAccountBuilder(LinkAccount.forPatient(patient))

    fun linkageKeyPOSTRequest(linkAccount: LinkAccount) = TppLinkagePOSTBuilder(linkAccount)

    fun linkageKeyGetRequest(linkAccount: LinkAccount)= TppLinkageGETBuilder (linkAccount)
}

