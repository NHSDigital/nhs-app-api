package features.linkage.stepDefinitions

import cucumber.api.java.en.*
import mocking.MockingClient
import mocking.defaults.MockDefaults.Companion.DEFAULT_ODS_CODE
import mocking.emis.models.GetLinkageResponse
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

open class LinkageStepDefinitions {

    val HTTP_EXCEPTION = "HttpException"

    private val EMIS = "EMIS"
    lateinit var currentGPSystem: String
    lateinit var odsCode: String
    lateinit var nhsNumber: String
    lateinit var accountId: String
    private var gpSystemHttpStatusResponseCode: Int = HttpStatus.SC_OK
    private var linkageKey: String = Patient.johnSmith.linkageKey
    private var linkageResponse: LinkageResponse? = null

    val mockingClient = MockingClient.instance

    @Given("^I have a valid (.*) OdsCode$")
    fun ihaveAValidXOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem

        when (currentGPSystem) {
            EMIS -> {
                odsCode = DEFAULT_ODS_CODE
            }
        }
    }

    @Given("^I have an invalid (.*) OdsCode$")
    fun ihaveAnInvalidXOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem
        odsCode = ""
    }

    @Given("^I have a not found (.*) OdsCode$")
    fun ihaveANotFoundXOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem
        odsCode = "A04889999"
    }

    @And("^I have a valid NhsNumber$")
    fun ihaveAnValidNhsNumber() {
        when (currentGPSystem) {
            EMIS -> {
                nhsNumber = Patient.johnSmith.nhsNumbers.get(0)
            }
        }
    }

    @And("^I have an invalid NhsNumber$")
    fun ihaveAnInvalidNhsNumber() {
        nhsNumber = ""
    }

    @And("^I have a not found NhsNumber$")
    fun iHaveANotFoundNhsNumber() {
        gpSystemHttpStatusResponseCode = HttpStatus.SC_NOT_FOUND
        nhsNumber = "rtete345343"
    }

    @And("My linkage key has been revoked")
    fun myLinkageKeyHasBeenRevoked() {
        gpSystemHttpStatusResponseCode = HttpStatus.SC_FORBIDDEN
    }

    @When("^I call the Linkage GET endpoint$")
    fun iCallTheLinkageGETEndpoint() {
        when (currentGPSystem) {
            EMIS -> {
                linkageKey = Patient.johnSmith.linkageKey
                accountId = Patient.johnSmith.accountId
                when (gpSystemHttpStatusResponseCode) {
                    HttpStatus.SC_NOT_FOUND -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(nhsNumber, odsCode).respondWithNotFoundException()
                        }
                    }
                    HttpStatus.SC_OK -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(nhsNumber, odsCode).respondWithSuccess(GetLinkageResponse(odsCode, linkageKey, accountId))
                        }
                    }
                    HttpStatus.SC_FORBIDDEN -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(nhsNumber, odsCode).respondWithForbiddenException()
                        }
                    }
                }
            }
        }

        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getLinkageKey(nhsNumber, odsCode)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("I call the Linkage POST endpoint")
    fun iCallTheLinkagePOSTEndpoint() {
        when (currentGPSystem) {
            EMIS -> {
                linkageKey = Patient.johnSmith.linkageKey
                accountId = Patient.johnSmith.accountId
                when (gpSystemHttpStatusResponseCode) {
                    HttpStatus.SC_NOT_FOUND -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(CreateLinkageRequest(odsCode, nhsNumber))
                                    .respondWithNotFoundException()
                        }
                    }
                    HttpStatus.SC_OK -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(CreateLinkageRequest(odsCode, nhsNumber))
                                    .respondWithSuccess(GetLinkageResponse(odsCode, linkageKey, accountId))
                        }
                    }
                    HttpStatus.SC_CONFLICT -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(CreateLinkageRequest(odsCode, nhsNumber))
                                    .respondWithConflictException()
                        }
                    }
                }
            }
        }

        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).postLinkageKey(CreateLinkageRequest(odsCode, nhsNumber))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }

    }

    @When("^I receive a valid response$")
    fun iReceiveAValidResponse() {
        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(odsCode, linkageResponse!!.odsCode)
        Assert.assertEquals(accountId, linkageResponse!!.accountId)
        Assert.assertEquals(linkageKey, linkageResponse!!.linkageKey)
    }

    @But("A linkage key already exists for the user")
    fun aLinkageKeyAlreadyExistsForTheUser() {
        gpSystemHttpStatusResponseCode = HttpStatus.SC_CONFLICT
    }
}


