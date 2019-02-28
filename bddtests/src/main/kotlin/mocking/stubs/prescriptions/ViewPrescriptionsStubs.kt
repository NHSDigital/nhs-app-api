package mocking.stubs.prescriptions

import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.spine.ePS.models.*
import mocking.spine.ePS.prescriptions.EPS111ItemDetailBuilder
import mocking.spine.ePS.prescriptions.EPS111ItemSummaryBuilder
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.prescriptions.EmisPrescriptionsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class ViewPrescriptionsStubs(private val mockingClient: MockingClient) {

    companion object {
        private const val NUMBER_OF_PRESCRIPTIONS = 5
        private const val NUMBER_OF_COURSES = 5
        private const val NUMBER_OF_REPEAT_PRESCRIPTIONS = 5
    }
    fun generateEMISStubs() {
        val loadEMISPrescriptions = prescriptionLoaderEMIS()
        val mapEMISViewPrescriptionRequestStubs =
                InputResponse<Patient, EmisPrescriptionsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(loadEMISPrescriptions)
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithPrescriptionsNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(loadEMISPrescriptions)
                                    .whenScenarioStateIs("Started")
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISViewPrescriptionRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(prescriptions.prescriptionsRequest(scenario.forMatcher)) }
        }
    }

    fun generateSpineStubs() {
        val loadSpinePrescription = prescriptionLoaderSpine()
        val loadSpinePrescriptionDetail1 = prescriptionDetail1LoadedSpine()
        val loadSpinePrescriptionDetail2 = prescriptionDetail2LoadedSpine()
        val loadSpinePrescriptionDetail3 = prescriptionDetail3LoadedSpine()

        val mapSpinePrescriptionTrackingStubs =
                InputResponse<Patient, EPS111ItemSummaryBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(loadSpinePrescription)
                                    .whenScenarioStateIs("Started")
                        }
        val mapSpinePrescriptionDetailStubs =
                InputResponse<String, EPS111ItemDetailBuilder>()
                        .addResponse("2D35F7-ZA0448-11E88Z") { builder
                            ->
                            builder.respondWithSuccess(loadSpinePrescriptionDetail1)
                                    .whenScenarioStateIs("Started")
                        }
                        .addResponse("ABC5F7-ZA0448-77E88X") { builder
                            ->
                            builder.respondWithSuccess(loadSpinePrescriptionDetail2)
                                    .whenScenarioStateIs("Started")
                        }
                        .addResponse("ABC5F7-ZA0448-77E89X") { builder
                            ->
                            builder.respondWithSuccess(loadSpinePrescriptionDetail3)
                                    .whenScenarioStateIs("Started")
                        }

        mapSpinePrescriptionTrackingStubs.listResponse().forEach { scenario ->
            mockingClient.forSpine {
                scenario.getResponse(itemSummary.prescriptionTrackingRequest())
            }
        }

        mapSpinePrescriptionDetailStubs.listResponse().forEach { scenario ->
            mockingClient.forSpine { scenario.getResponse(itemDescription.prescriptionDetailTrackingRequest(scenario.forMatcher)) }
        }
    }

    private fun prescriptionLoaderSpine(): SpineItemSummaryGetResponse {

        val sp1 = SpineItemSummaryPrescription(
                lastEventDate = "20180422095703",
                prescriptionIssueDate = "20180117095703",
                patientNhsNumber = "9912003489",
                epsVersion = "R2",
                repeatInstance = RepeatInstance("2", "6"),
                pendingCancellations = "False",
                prescriptionTreatmentType = "Repeat Prescribing Prescription",
                prescriptionStatus = "Dispensed",
                lineItems = mapOf(
                        "30b7e9cf-6f42-40a8-84c1-e61ef638eee2" to "Perindopril erbumine 2mg tablets",
                        "636f1b57-e18c-4f45-acae-2d7db86b6e1e" to "Metformin 500mg modified-release tablets"
                )
        )

        val sp2 = SpineItemSummaryPrescription(
                lastEventDate = "20180319115010",
                prescriptionIssueDate = "20180319101307",
                patientNhsNumber = "9912003489",
                epsVersion = "R2",
                currentIssueNumber = "1",
                pendingCancellations = "False",
                prescriptionTreatmentType = "Repeat Dispensing Prescription",
                prescriptionStatus = "Dispensed",
                lineItems = mapOf(
                        "636f1b57-e18c-4f45-acae-2d7db86b6e1e" to "Hydrocortisone 0.5% cream"
                )
        )

        val sp3 = SpineItemSummaryPrescription(
                lastEventDate = "20180423095703",
                prescriptionIssueDate = "20180118095703",
                patientNhsNumber = "9912003489",
                epsVersion = "R2",
                repeatInstance = RepeatInstance("2", "6"),
                pendingCancellations = "False",
                prescriptionTreatmentType = "Repeat Prescribing Prescription",
                prescriptionStatus = "Dispensed",
                lineItems = mapOf(
                        "30b7e9cf-6f42-40a8-84c1-e61ef638eee2" to "Condine 10mg tablets",
                        "636f1b57-e18c-4f45-acae-2d7db86b6e1e" to "Paracetamol 500mg modified-release tablets",
                        "edfee2fe-643e-4f96-ad61-400217508826" to "Calpol 500ml solution",
                        "dccb4a5c-506a-4509-b630-9c488334b937" to "Metformin 500mg modified-release tablets"
                )
        )

        return SpineItemSummaryGetResponse(
                version = "1",
                reason = "",
                statusCode = "0",
                prescriptions = mapOf(
                        "2D35F7-ZA0448-11E88Z" to sp1,
                        "ABC5F7-ZA0448-77E88X" to sp2,
                        "ABC5F7-ZA0448-77E89X" to sp3
                )
        )
    }

    private fun prescriptionDetail1LoadedSpine(): SpineItemDetailGetResponse {

        val lineItemDetail1 = LineItemDetail(code = "193611000001107",
                quantity = "84", itemStatus = "Not Dispensed",
                dosage = "As Directed", description = "Micronor 350microgram tablets (Janssen-Cilag Ltd)",
                uom = "tablet")
        val lineItemDetail2 = LineItemDetail(code = "330987003", dosage = "As Directed",
                quantity = "10", itemStatus = "Dispensed", description = "Sodium bicarbonate 5% ear drops",
                uom = "ml")
        val mainPharmacy = Organisation(address = "The Pharmacy, The Street, SW8 3QJ",
                name = "Test Pharmacy", ods = "FA00D", phone = "07775123456")

        val sp1 = SpineItemDetailPrescription(
                pendingCancellations = "false",
                prescriptionLastIssueDispensedDate = "false",
                prescriptionDownloadDate = "20190213150209",
                repeatInstance = RepeatInstance(currentIssue = "1", totalAuthorised = "1"),
                prescriptionDispensedDate = "20190205",
                prescriptionTreatmentType = "Repeat Prescribing",
                lastEventDate = "20190215105008",
                prescriptionClaimedDate = "20190215105008",
                lineItems = mapOf(
                        "30b7e9cf-6f42-40a8-84c1-e61ef638eee2" to lineItemDetail1,
                        "636f1b57-e18c-4f45-acae-2d7db86b6e1e" to lineItemDetail2),
                nominatedPharmacy = mainPharmacy,
                patientNhsNumber = "9651614498",
                prescriber = Organisation(address = "13 VERNON STREET, DERBY, DERBYSHIRE, DE1 1FW",
                        name = "VERNON STREET MEDICAL CTR", phone = "01512631737", ods = "C81007"),
                epsVersion = "R2",
                prescriptionIssueDate = "20180907120900",
                prescriptionStatus = "Claimed",
                dispensingPharmacy = mainPharmacy

        )

        return SpineItemDetailGetResponse(
                statusCode = "0",
                reason = "",
                version = "1",
                prescriptions = mapOf("2D35F7-ZA0448-11E88Z" to sp1)
        )
    }

    private fun prescriptionDetail2LoadedSpine() : SpineItemDetailGetResponse {

        val lineItemDetail3 = LineItemDetail(code = "159411000001102", dosage = "As Directed",
                quantity = "250", itemStatus = "Dispensed", uom = "ml",
                description = "Oilatum Emollient (GlaxoSmithKline Consumer Healthcare)")
        val lineItemDetail4 = LineItemDetail(code = "28426011000001108",
                dosage = "Inject subcutaneously as directed", quantity = "4", itemStatus = "Dispensed",
                uom = "pre-filled disposable injection",
                description = "Bydureon 2mg powder and solvent for " +
                        "prolonged-release suspension for injection pre-filled pen (AstraZeneca UK Ltd)")
        val mainPharmacy = Organisation(address = "The Pharmacy, The Street, SW8 3QJ",
                name = "Test Pharmacy", ods = "FA00D", phone = "07775123456")

        val sp1 = SpineItemDetailPrescription(
                pendingCancellations = "false",
                prescriptionLastIssueDispensedDate = "false",
                prescriptionDownloadDate = "20190213150209",
                repeatInstance = RepeatInstance(currentIssue = "1", totalAuthorised = "1"),
                prescriptionDispensedDate = "20190205",
                prescriptionTreatmentType = "Repeat Prescribing",
                lastEventDate = "20190215105008",
                prescriptionClaimedDate = "20190215105008",
                lineItems = mapOf(
                        "754B10D1-04D4-3828-E050-D20AE3A22BBD" to lineItemDetail3,
                        "754B10D1-04E4-3828-E050-D20AE3A22BBD" to lineItemDetail4),
                nominatedPharmacy = mainPharmacy,
                patientNhsNumber = "9651614498",
                prescriber = Organisation(address = "13 VERNON STREET, DERBY, DERBYSHIRE, DE1 1FW",
                        name = "VERNON STREET MEDICAL CTR", phone = "01512631737", ods = "C81007"),
                epsVersion = "R2",
                prescriptionIssueDate = "20180907120900",
                prescriptionStatus = "Claimed",
                dispensingPharmacy = mainPharmacy

        )

        return SpineItemDetailGetResponse(
                statusCode = "0",
                reason = "",
                version = "1",
                prescriptions = mapOf("54A9B7-C81007-000014" to sp1)
        )

    }

    private fun prescriptionDetail3LoadedSpine() : SpineItemDetailGetResponse {

        val lineItemDetail1 = LineItemDetail(code = "159411000001103", dosage = "As Directed",
                quantity = "250", itemStatus = "Dispensed", uom = "ml",
                description = "Oilatum Emollient (GlaxoSmithKline Consumer Healthcare)")
        val lineItemDetail2 = LineItemDetail(code = "28426011000001104",
                dosage = "Inject subcutaneously as directed", quantity = "4", itemStatus = "Dispensed",
                uom = "pre-filled disposable injection",
                description = "Bydureon 2mg powder and solvent for " +
                        "prolonged-release suspension for injection pre-filled pen (AstraZeneca UK Ltd)")
        val lineItemDetail3 = LineItemDetail(code = "159411000001102", dosage = "As Directed",
                quantity = "250", itemStatus = "Dispensed", uom = "ml",
                description = "Oilatum Emollient (GlaxoSmithKline Consumer Healthcare)")
        val lineItemDetail4 = LineItemDetail(code = "28426011000001108",
                dosage = "Inject subcutaneously as directed", quantity = "4", itemStatus = "Dispensed",
                uom = "pre-filled disposable injection",
                description = "Bydureon 2mg powder and solvent for " +
                        "prolonged-release suspension for injection pre-filled pen (AstraZeneca UK Ltd)")
        val mainPharmacy = Organisation(address =  "Stephens Pharm-A-C, Best Street, DERBY, DERBYSHIRE  SW8 3KJ",
                name = "Stephens Pharm-A-C", ods = "FA00D", phone = "07775123456")

        val sp1 = SpineItemDetailPrescription(
                pendingCancellations = "false",
                prescriptionLastIssueDispensedDate = "false",
                prescriptionDownloadDate = "20190213150209",
                repeatInstance = RepeatInstance(currentIssue = "1", totalAuthorised = "1"),
                prescriptionDispensedDate = "20190205",
                prescriptionTreatmentType = "Repeat Prescribing",
                lastEventDate = "20190215105008",
                prescriptionClaimedDate = "20190215105008",
                lineItems = mapOf(
                        "754B10D1-04D4-3828-E050-D20AE3A22BBD" to lineItemDetail1,
                        "754B10D1-04E4-3828-E050-D20AE3A22BAD" to lineItemDetail2,
                        "754B10D1-04D4-3828-E050-D20AE3A22BCD" to lineItemDetail3,
                        "754B10D1-04E4-3828-E050-D20AE3A22BDD" to lineItemDetail4),
                nominatedPharmacy = mainPharmacy,
                patientNhsNumber = "9651614498",
                prescriber = Organisation(address = "13 VERNON STREET, DERBY, DERBYSHIRE, DE1 1FW",
                        name = "VERNON STREET MEDICAL CTR", phone = "01512631737", ods = "C81007"),
                epsVersion = "R2",
                prescriptionIssueDate = "20180907120900",
                prescriptionStatus = "Claimed",
                dispensingPharmacy = mainPharmacy

        )

        return SpineItemDetailGetResponse(
                statusCode = "0",
                reason = "",
                version = "1",
                prescriptions = mapOf("ABC5F7-ZA0448-77E89X" to sp1)
        )

    }

    private fun prescriptionLoaderEMIS(): PrescriptionRequestsGetResponse {
        val prescriptionLoader = EmisPrescriptionLoader
        prescriptionLoader.loadData(
                noPrescriptions = NUMBER_OF_PRESCRIPTIONS,
                noCourses = NUMBER_OF_COURSES,
                noRepeats = NUMBER_OF_REPEAT_PRESCRIPTIONS,
                showDosage = true,
                showQuantity = true
        )

        return prescriptionLoader.data
    }
}