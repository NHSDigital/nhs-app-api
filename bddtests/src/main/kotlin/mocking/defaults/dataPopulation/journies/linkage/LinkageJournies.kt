package mocking.defaults.dataPopulation.journies.linkage

import mocking.MockingClient
import mocking.emis.models.GetLinkageResponse
import worker.models.linkage.CreateLinkageRequest
import java.time.Duration

class LinkageJournies(private val client: MockingClient) {

    fun create() {
        createGetJournies()
        createPostJournies()
    }

    private fun createPostJournies() {

        val createPatient = Linkage("A29928", "3336669990", "vVGO567gV6fvPb", "675234" )
        client.forEmis {
                linkageKeyPOSTRequest(
                        CreateLinkageRequest(createPatient.odsCode, createPatient.nhsNumber))
                        .respondWithSuccess(GetLinkageResponse(
                                createPatient.odsCode,
                                createPatient.linkageKey!!,
                                createPatient.accountId!!
                        ))
        }

        val notFoundPatient = Linkage("A29928", "4447770001")
        client.forEmis {
                linkageKeyPOSTRequest(
                        CreateLinkageRequest(notFoundPatient.odsCode, notFoundPatient.nhsNumber))
                        .respondWithNotFoundException()
        }

        val conflictPatient = Linkage("A29928", "5558881112")
        client.forEmis {
                linkageKeyPOSTRequest(
                        CreateLinkageRequest(conflictPatient.odsCode, conflictPatient.nhsNumber))
                        .respondWithConflictException()
        }

        val badGatewayPatient = Linkage("A29928", "5634234345")
        client.forEmis {
            linkageKeyPOSTRequest(
                    CreateLinkageRequest(badGatewayPatient.odsCode, badGatewayPatient.nhsNumber))
                    .respondWithBadGatewayException()
        }

        val timeoutPatient = Linkage("A29928", "5634200045", "tTALtBP3rLR16", "542343")
        client.forEmis {
            linkageKeyPOSTRequest(
                    CreateLinkageRequest(timeoutPatient.odsCode, timeoutPatient.nhsNumber))
                    .respondWithSuccess(GetLinkageResponse(timeoutPatient.odsCode, timeoutPatient.linkageKey!!, timeoutPatient.accountId!!))
                    .delayedBy( Duration.ofSeconds(15))
        }

    }

    private fun createGetJournies() {

        val existingPatientLinkages = listOf(
                Linkage("A29928", "3434234345", "tTALtBP3rLR16", "542343"),
                Linkage("A29928", "5454253356", "vVGO8bgV6fvPb", "897348")
        )

        for (linkage in existingPatientLinkages) {
            client.forEmis {
                linkageKeyGetRequest(linkage.nhsNumber, linkage.odsCode)
                        .respondWithSuccess(GetLinkageResponse(linkage.odsCode, linkage.linkageKey!!, linkage.accountId!!))
            }
        }

        val expiredPatientLinkages = listOf(
                Linkage("A29928", "4642234432", "Bmij89KnhY8Jp", "643243"),
                Linkage("A29928", "6423432552", "NAw3hSsw87hu2", "343555")
        )

        for (linkage in expiredPatientLinkages) {
            client.forEmis {
                linkageKeyGetRequest(linkage.nhsNumber, linkage.odsCode)
                        .respondWithForbiddenException()
            }
        }

        val notFoundPatient = Linkage("A29928", "3434994345")
        client.forEmis {
                linkageKeyGetRequest(notFoundPatient.nhsNumber, notFoundPatient.odsCode)
                        .respondWithNotFoundException()
        }

        val invalidODSCode = Linkage("C94839", "3434234345")
        client.forEmis {
            linkageKeyGetRequest(invalidODSCode.nhsNumber, invalidODSCode.odsCode)
                    .respondWithNotImplementedException()
        }

        val badGatewayPatient = Linkage("A29928", "5634234345")
        client.forEmis {
            linkageKeyGetRequest(badGatewayPatient.nhsNumber, badGatewayPatient.odsCode)
                    .respondWithBadGatewayException()
        }

        val timeoutPatient = Linkage("A29928", "5634200045", "tTALtBP3rLR16", "542343")
        client.forEmis {
            linkageKeyGetRequest(timeoutPatient.nhsNumber, timeoutPatient.odsCode)
                    .respondWithSuccess(GetLinkageResponse(timeoutPatient.odsCode, timeoutPatient.linkageKey!!, timeoutPatient.accountId!!))
                    .delayedBy( Duration.ofSeconds(15))
        }







    }
}