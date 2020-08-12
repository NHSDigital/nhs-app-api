package features.prescriptions.stepDefinitions

import constants.Supplier
import io.cucumber.datatable.DataTable
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.MicrotestPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.mappers.VisionPrescriptionMapper
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.defaults.EmisMockDefaults
import mocking.defaults.VisionMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourseStatus
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.vision.models.PrescriptionHistory
import models.Patient
import models.prescriptions.HistoricPrescription
import models.prescriptions.PrescriptionLoaderConfiguration
import org.junit.Assert.assertTrue
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.prescription.RepeatPrescriptionConfirmationPage
import pages.prescription.RepeatPrescriptionsPage
import pages.prescription.ViewOrdersPrescriptionsPage
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set
import java.time.OffsetDateTime

private const val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY = 6L
private const val DATE_GREATER_THAN_SIX_MONTHS = 7L
private const val NUM_OF_PRESCRIPTIONS = 10

open class PrescriptionsStepDefinitions {

    private lateinit var viewOrders : ViewOrdersPrescriptionsPage
    private lateinit var repeatPrescriptions: RepeatPrescriptionsPage
    private lateinit var repeatPrescriptionConfirmation : RepeatPrescriptionConfirmationPage

    private val mockingClient = MockingClient.instance

    private val toDate = OffsetDateTime.now()
    private var numberOfPrescriptions: Int = 0
    private var numOfCourses: Int = 0
    private var numOfRepeats: Int = 0

    @Given("^I have no repeat prescriptions$")
    fun givenIHaveNoRepeatPrescriptions() {
        PrescriptionsDataSetup.initialize(SerenityHelpers.getGpSupplier())
        givenIHaveXPastRepeatPrescriptions(0)
        givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(0, 0)
    }

    @Given("^each repeat prescription contains (\\d+) courses of which (\\d+) are repeats$")
    fun givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(numOfCourses: Int, numOfRepeats: Int) {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        this.numOfCourses = numOfCourses
        this.numOfRepeats = numOfRepeats

        PrescriptionsDataSetup.setupWiremockAndData(expectedDefaultFromDate,
                toDate,
                numOfCourses * numberOfPrescriptions,
                numOfRepeats * numberOfPrescriptions,
                numberOfPrescriptions)
    }

    @Given("^each repeat prescription shares the same course")
    fun givenEachRepeatPrescriptionSharesTheSameCourse() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        numOfCourses = 1
        numOfRepeats = 1

        PrescriptionsDataSetup.setupWiremockAndData(
                expectedDefaultFromDate,
                toDate,
                numOfCourses,
                numOfRepeats,
                numberOfPrescriptions)
    }

    @Given("^I am patient using the (.*) GP System$")
    fun givenIAmAPatientUsingGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setGpSupplier(supplier)
        val currentPatient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(currentPatient)
        CitizenIdSessionCreateJourney().createFor(currentPatient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(currentPatient)
        PrescriptionsDataSetup.initialize(supplier)
    }

    @Given("^From date is 6 months ago and I have 10 prescriptions in the last 6 months$")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        numberOfPrescriptions = NUM_OF_PRESCRIPTIONS
        numOfRepeats = NUM_OF_PRESCRIPTIONS
        numOfCourses = NUM_OF_PRESCRIPTIONS

        PrescriptionsDataSetup.setupWiremockAndData(expectedDefaultFromDate,
                toDate,
                numOfCourses,
                numOfRepeats,
                numberOfPrescriptions)
    }

    @Given("^I have (\\d+) past repeat prescriptions$")
    fun givenIHaveXPastRepeatPrescriptions(numPrescriptions: Int) {
        numberOfPrescriptions = numPrescriptions
    }

    @Given("^a fromDate in an unexpected format$")
    fun aFromDateInAnUnexpectedFormat() {
        PrescriptionsSerenityHelpers.FROM_DATE.set("13/66/99999999T")

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @Given("^a fromDate in the future$")
    fun aFromDateInTheFuture() {
        PrescriptionsSerenityHelpers.FROM_DATE.set( toDate.plusMonths(1).toString())

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @Given("^a fromDate greater than 6 months ago$")
    fun aFromDateGreaterThanSixMonths() {
        PrescriptionsSerenityHelpers.FROM_DATE.set( toDate.minusMonths(DATE_GREATER_THAN_SIX_MONTHS).toString())

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @Given("^the GP System has disabled prescriptions$")
    fun theGPSystemHasDisabledPrescriptions() {
        var currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        if (currentProvider == null) {
            PrescriptionsDataSetup.initialize(SerenityHelpers.getGpSupplier())
            currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        }
        PrescriptionsFactory.getForSupplier(currentProvider!!).disableAtGPLevel()
    }

    @Given("^the GP System session has expired when viewing prescriptions$")
    fun theGPSystemSessionHasExpired() {
        var currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        if (currentProvider == null) {
            PrescriptionsDataSetup.initialize(SerenityHelpers.getGpSupplier())
            currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        }
        PrescriptionsFactory.getForSupplier(currentProvider!!).gpSessionHasExpired()
    }

    @Given("^prescriptions is disabled at a GP Practice level$")
    fun prescriptionsIsDisabledAtAGPLevel() {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        val currentPatient = SerenityHelpers.getPatient()
        PrescriptionsDataSetup.disabled(currentPatient, currentProvider!!)
    }

    @Given("^prescriptions is disabled for the proxy account at a GP Practice level$")
    fun prescriptionsIsDisabledAtAGPLevelForProxy() {
        PrescriptionsDataSetup.disabled(ProxySerenityHelpers.getPatientOrProxy(), Supplier.TPP)
    }

    @Given("^the prescription was ordered by proxy user$")
    fun thePrescriptionWasOrderedByProxyUser() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        val prescriptionLoader = EmisPrescriptionLoader

        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
            noPrescriptions = numberOfPrescriptions,noCourses = numOfRepeats, noRepeats = numOfRepeats
            )


        prescriptionLoader.loadData(
               prescriptionLoaderConfig,
                true)

        mockingClient.forEmis.mock {
                    prescriptions.prescriptionsRequest(
                            EmisMockDefaults.patientEmis,
                            expectedDefaultFromDate,
                            toDate)
                            .respondWithSuccess(prescriptionLoader.data)
                }
    }

    @Given("^each course has (.*)$")
    fun eachCourseHasX(contents: String) {
        val showDosage = contents.toLowerCase().contains("dosage")
        val showQuantity = contents.toLowerCase().contains("quantity")

        val currentPatient = SerenityHelpers.getPatient()
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        val prescriptionLoader = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LOADER.getOrFail<IPrescriptionLoader<*>>()


        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                noPrescriptions = numberOfPrescriptions,noCourses = numOfRepeats,
                noRepeats = numOfRepeats, showDosage = showDosage,
                showQuantity = showQuantity
        )


        prescriptionLoader.loadData(prescriptionLoaderConfig)

        when (currentProvider) {
            Supplier.EMIS -> {

                mockingClient.forEmis.mock {
                            prescriptions.prescriptionsRequest(
                                    EmisMockDefaults.patientEmis,
                                    expectedDefaultFromDate,
                                    toDate)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionRequestsGetResponse)
                        }
            }
            Supplier.TPP -> {

                mockingClient.forTpp.mock {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                        }
            }
            Supplier.VISION -> {

                mockingClient.forVision.mock {
                            prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                        }
            }
            Supplier.MICROTEST -> {

                mockingClient
                        .forMicrotest.mock {
                            prescriptions.getPrescriptionHistoryRequest(currentPatient, expectedDefaultFromDate)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistoryGetResponse)
                        }
            }
        }
    }

    @Given("^courses have status$")
    fun givenCoursesHaveStatus(statuses: DataTable) {

        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        var statusIndex = 0
        val data = statuses.cells()

        val prescriptionLoader = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LOADER.getOrFail<IPrescriptionLoader<*>>()
        val prgr = prescriptionLoader.data as PrescriptionRequestsGetResponse

        for (prescription in prgr.prescriptionRequests) {
            for (course in prescription.requestedMedicationCourses) {
                val status = data.get(statusIndex).get(0)
                course.requestedMedicationCourseStatus = RequestedMedicationCourseStatus.valueOf(status)
                statusIndex++
            }
        }

        mockingClient.forEmis.mock {
                    prescriptions.prescriptionsRequest(
                            EmisMockDefaults.patientEmis,
                            expectedDefaultFromDate, toDate)
                            .respondWithSuccess(prgr)
                }
    }

    @When("^I do not request a fromDate$")
    fun iDoNotRequestAFromDate() {
        PrescriptionsSerenityHelpers.FROM_DATE.set(null)
    }

    @When("^the GP System is too slow$")
    fun butTheGPSystemIsTooSlow() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        numberOfPrescriptions = 1
        numOfRepeats = 1
        numOfCourses = 1

        PrescriptionsDataSetup.setupWiremockAndDataWithDelay(fromdate = expectedDefaultFromDate,
                toDate = toDate,
                numberOfPrescriptions = numberOfPrescriptions,
                numOfRepeats = numOfRepeats )
    }

    @Then("^I see the name of the proxy user who ordered the prescription$")
    fun thenISeeTheNameOfTheProxyUseWhoOrderedThePrescription() {
        viewOrders.orderedByLabel.assertIsVisible()
    }

    @Then("^I click the change nominated pharmacy link on the view orders page$")
    fun thenIClickTheChangeNominatedPharmacyLink() {
        viewOrders.iClickTheChangeNominatedPharmacyLink()
    }

    @Then("^I do not see the name of the proxy user who ordered the prescription$")
    fun thenIDoNotSeeTheNameOfTheProxyUseWhoOrderedThePrescription() {
        viewOrders.orderedByLabel.assertElementNotPresent()
    }

    @Then("^I see a message indicating that I have no repeat prescriptions$")
    fun thenISeeAMessageIndicatingThatIHaveNoRepeatPrescriptions() {
        assertTrue(viewOrders.isNoPrescriptionsMessageVisible())
    }

    @Then("^I see (\\d+) prescriptions$")
    fun thenISeeXPrescriptions(numPrescriptions: Int) {
        viewOrders
                .assertPrescriptionsMatch(
                        getResponseToExpectedPrescriptionFormat(),
                        numPrescriptions,
                        providerHasAllPrescriptionFields())
    }

    @Then("^I see view orders prescriptions page loaded$")
    fun iSeeViewOrdersPrescriptionsPageLoaded() {
        viewOrders.isLoaded()
    }

    @Then("^I see repeat prescription confirmation page loaded$")
    fun iSeeRepeatPrescriptionConfirmationPageLoaded() {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        if (currentProvider === Supplier.TPP) {
            repeatPrescriptionConfirmation.isLoaded(ProxySerenityHelpers.getPatientOrProxy().formattedFullName())
        } else {
            repeatPrescriptionConfirmation.isLoaded(ProxySerenityHelpers.getPatientOrProxy().name.firstName)
        }
    }

    @Then("^I see no prescriptions$")
    fun iSeeNoPrescriptions() {
        assertTrue(viewOrders.isNoPrescriptionsMessageVisible())
    }

    private fun providerHasAllPrescriptionFields(): Boolean {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        return currentProvider == Supplier.EMIS ||
                currentProvider == Supplier.VISION ||
                currentProvider == Supplier.MICROTEST
    }

    @Then("^I select (\\d+) prescription to order$")
    fun iSelectXPrescriptionsToOrder(prescriptionToOrder: Int) {
        repeatPrescriptions.selectXPrescriptionsToOrder(prescriptionToOrder)
        repeatPrescriptions.orderRepeatPrescriptionButton.click()
        repeatPrescriptions.clickConfirmAndOrderRepeatSubscriptionButton()
    }

    private fun getResponseToExpectedPrescriptionFormat(): List<HistoricPrescription> {

        val prescriptionLoader = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LOADER.getOrFail<IPrescriptionLoader<*>>()
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        return when (currentProvider) {
            Supplier.EMIS ->
                EmisPrescriptionMapper.map(prescriptionLoader.data as PrescriptionRequestsGetResponse)
            Supplier.TPP ->
                TppPrescriptionMapper.map(prescriptionLoader.data as ListRepeatMedicationReply)
            Supplier.VISION ->
                VisionPrescriptionMapper.map(prescriptionLoader.data as PrescriptionHistory)
            Supplier.MICROTEST ->
                MicrotestPrescriptionMapper.map(prescriptionLoader.data as PrescriptionHistoryGetResponse)
            else -> throw NotImplementedError()
        }
    }

    private fun getDefaultPrescriptionsFromDate(dateNow: OffsetDateTime): OffsetDateTime {
        return dateNow.minusMonths(PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY)
    }
}
