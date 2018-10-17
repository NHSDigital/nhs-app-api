package mocking.defaults.dataPopulation.journies.im1Connection

import mocking.MockingClient
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import models.Patient

class NoOnlineUserFoundJourney(private val client: MockingClient) {
    val patient = Patient.halleDawe

    fun create() {
        client
            .forEmis {
                authentication.endUserSessionRequest()
                        .respondWithSuccess(patient.endUserSessionId)
            }

        client
            .forEmis {
                authentication.meApplicationsRequest(
                        patient,
                        LinkApplicationRequestModel(
                                surname = patient.surname,
                                dateOfBirth = patient.dateOfBirth,
                                linkageDetails = LinkageDetailsModel(
                                        accountId = patient.accountId,
                                        linkageKey = patient.linkageKey,
                                        nationalPracticeCode = patient.odsCode
                                )
                        )
                ).respondWithNoOnlineUserFound()
            }
    }
}