package mocking.defaults.dataPopulation.journies.linkage

import mocking.MockingClient
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.AddVerificationResponse
import mocking.emis.models.AddVerificationRequest
import worker.models.linkage.CreateLinkageRequest
import java.time.Duration

private const val TIMEOUT_DURATION_SECONDS:Long = 15

class LinkageJournies(private val client: MockingClient) {


    fun create() {
        createGetJournies()
        createPostJournies()
    }

    private fun createPostJournies() {

        val createPatient = Linkage("A29928", "3336669990", "vVGO567gV6fvPb", "675234", identityToken = "idToken")
        client.forEmis {
                linkageKeyPOSTRequest(
                        AddNhsUserRequest(createPatient.odsCode, createPatient.nhsNumber, createPatient.emailAddress))
                        .respondWithSuccessfullyCreated(AddNhsUserResponse(""))
        }

        val notFoundPatient = Linkage("A29928", "4447770001", "notFound@email.com", identityToken = "idToken")
        client.forEmis {
                linkageKeyPOSTRequest(
                        AddNhsUserRequest(notFoundPatient.odsCode, notFoundPatient.nhsNumber, notFoundPatient.emailAddress))
                        .respondWithNoRegisteredOnlineUserFound()
        }

        val conflictPatient = Linkage("A29928", "5558881112", "conflict@email.com", identityToken = "idToken")
        client.forEmis {
                linkageKeyPOSTRequest(
                        AddNhsUserRequest(conflictPatient.odsCode, conflictPatient.nhsNumber, notFoundPatient.emailAddress))
                        .respondWithPatientAlreadyHasAnOnlineAccount()
        }

        val badGatewayPatient = Linkage("A29928", "5634234345", "badgateway@email.com", identityToken = "idToken")
        client.forEmis {
            linkageKeyPOSTRequest(
                    AddNhsUserRequest(badGatewayPatient.odsCode, badGatewayPatient.nhsNumber, notFoundPatient.emailAddress))
                    .respondWithBadGatewayException()
        }

        val timeoutPatient = Linkage("A29928", "5634200045", "timeout@email.com", "tTALtBP3rLR16", "542343", identityToken = "idToken")
        client.forEmis {
            linkageKeyPOSTRequest(
                    AddNhsUserRequest(timeoutPatient.odsCode, timeoutPatient.nhsNumber, notFoundPatient.emailAddress))
                    .respondWithSuccessfullyCreated(AddNhsUserResponse(""))
                    .delayedBy( Duration.ofSeconds(TIMEOUT_DURATION_SECONDS))
        }
    }

    private fun createGetJournies() {

        val existingPatientLinkages = listOf(
                Linkage("A29928", "3434234345", "", "tTALtBP3rLR16", "542343"),
                Linkage("A29928", "5454253356", "", "vVGO8bgV6fvPb", "897348")
        )

        for (linkage in existingPatientLinkages) {
            client.forEmis {
                linkageKeyGetRequest(AddVerificationRequest(linkage.nhsNumber, linkage.odsCode, linkage.emailAddress))
                        .respondWithSuccessfullyRetrieved(AddVerificationResponse(linkage.odsCode, linkage.linkageKey!!, linkage.accountId!!))
            }
        }

        val expiredPatientLinkages = listOf(
                Linkage("A29928", "4642234432", "Bmij89KnhY8Jp", "643243"),
                Linkage("A29928", "6423432552", "NAw3hSsw87hu2", "343555")
        )

        for (linkage in expiredPatientLinkages) {
            client.forEmis {
                linkageKeyGetRequest(AddVerificationRequest(linkage.nhsNumber, linkage.odsCode, linkage.emailAddress))
                        .respondWithForbiddenException()
            }
        }

        val notFoundPatient = Linkage("A29928", "3434994345", "")
        client.forEmis {
                linkageKeyGetRequest(AddVerificationRequest(notFoundPatient.nhsNumber, notFoundPatient.odsCode, notFoundPatient.emailAddress))
                        .respondWithNoRegisteredOnlineUserFound()
        }

        val invalidODSCode = Linkage("C94839", "3434234345", "")
        client.forEmis {
            linkageKeyGetRequest(AddVerificationRequest(invalidODSCode.nhsNumber, invalidODSCode.odsCode, invalidODSCode.emailAddress))
                    .respondWithNotImplementedException()
        }

        val badGatewayPatient = Linkage("A29928", "5634234345", "")
        client.forEmis {
            linkageKeyGetRequest(AddVerificationRequest(badGatewayPatient.nhsNumber, badGatewayPatient.odsCode, badGatewayPatient.emailAddress))
                    .respondWithBadGatewayException()
        }

        val timeoutPatient = Linkage("A29928", "5634200045", "tTALtBP3rLR16", "542343", "14232")
        client.forEmis {
            linkageKeyGetRequest(AddVerificationRequest(timeoutPatient.nhsNumber, timeoutPatient.odsCode, timeoutPatient.emailAddress))
                    .respondWithSuccessfullyRetrieved(AddVerificationResponse(timeoutPatient.odsCode, timeoutPatient.linkageKey!!, timeoutPatient.accountId!!))
                    .delayedBy( Duration.ofSeconds(TIMEOUT_DURATION_SECONDS))
        }
    }
}
