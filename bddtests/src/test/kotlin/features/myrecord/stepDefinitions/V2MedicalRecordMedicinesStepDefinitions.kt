package features.myrecord.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.myrecord.factories.MedicationsFactory
import mocking.data.myrecord.MedicationsData
import org.junit.Assert
import pages.gpMedicalRecord.MedicinesDetailPage
import pages.gpMedicalRecord.MedicinesIndexPage
import utils.SerenityHelpers
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

open class V2MedicalRecordMedicinesStepDefinitions {

    private lateinit var medicinesIndexPage: MedicinesIndexPage
    private lateinit var medicinesDetailPage: MedicinesDetailPage

    private val expectedAcuteMedicinesData = mapOf(
        Supplier.EMIS to arrayOf(
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nPenicillin\nOne to be taken four times a day\n" +
                        "28 Capsules",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen\nOne to be taken twice a day\n" +
                        "14 Capsules"
        ), Supplier.VISION to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
        ), Supplier.TPP to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nVentolin"
        ), Supplier.MICROTEST to arrayOf(
                "27 March 2019\nMedication 3\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
        )
    )

    private val expectedCurrentMedicinesData = mapOf(
        Supplier.EMIS to arrayOf(
                "${formatDate(MedicationsData.twoMonthsAgo)}\nPainkiller Cocktail\nMegaMix, consisting of:\n" +
                        "Cocodomol - 150ml\nPethidine - 200ml\n" +
                        "5ml to be taken once a day\n350ml",
                "${formatDate(MedicationsData.threeMonthsAgo)}\nInhaler Mix\nMegaMix, consisting of:\n" +
                        "Ventolin - 150ml\nSalbutanol - 200ml\n" +
                        "One to be taken once a day\n2 inhalers",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nAmoxycillin\nOne to be taken twice a day\n" +
                        "14 Capsules",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen Extra\nOne to be taken once a day\n" +
                        "30 Capsules",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen Plus\nOne to be taken once a day\n" +
                        "7 Capsules"
        ), Supplier.VISION to arrayOf(
                "8 October 2018\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
        ), Supplier.TPP to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nVentolin",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nSalbutamol",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nCalpol"
        ), Supplier.MICROTEST to arrayOf(
                "27 March 2019\nMedication 1\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
        )
    )

    private val expectedDiscontinuedMedicinesData = mapOf(
            Supplier.EMIS to arrayOf(
                "${formatDate(MedicationsData.threeMonthsAgo)}\nAmoxycillin\nOne to be taken twice a day\n" +
                        "14 Capsules\nEnded: ${formatDate(MedicationsData.oneMonthAgo)}",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nCocodomol\nOne to be taken once a day\n" +
                        "7 Capsules\nEnded: ${formatDate(MedicationsData.twoMonthsAgo)}",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen\nOne to be taken once a day\n" +
                    "7 Capsules\nEnded: ${formatDate(MedicationsData.tenMonthsAgo)}"
            ), Supplier.VISION to arrayOf(
                "8 October 2018\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
            ), Supplier.TPP to arrayOf(
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nVentolin",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nSalbutamol",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nCalpol"
            ), Supplier.MICROTEST to arrayOf(
                "27 March 2019\nMedication 2\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
            )
    )

    @When("^I click the Acute medicines link - Medical Record v2$")
    fun whenIClickTheAcuteMedicinesLinkV2() {
        medicinesIndexPage.clickAcuteMedicinesLink()
    }

    @When("^I click the Current medicines link - Medical Record v2$")
    fun whenIClickTheCurrentMedicinesLinkV2() {
        medicinesIndexPage.clickCurrentMedicinesLink()
    }

    @When("^I click the Discontinued medicines link - Medical Record v2$")
    fun whenIClickTheDiscontinuedMedicinesLinkV2() {
        medicinesIndexPage.clickDiscontinuedMedicinesLink()
    }

    @Then("^I see the medical record v2 medicines page$")
    fun thenISeeTheMedicalRecordV2MedicinesPage(){
        medicinesIndexPage.assertIsVisible()
    }

    @Then("^I see the expected acute medicines - Medical Record v2$")
    fun thenISeeExpectedAcuteMedicinesV2() {
        assertExpectedMedicinesPresent(expectedAcuteMedicinesData)
    }

    @Then("^I see the expected current medicines - Medical Record v2$")
    fun thenISeeExpectedCurrentMedicinesV2() {
        assertExpectedMedicinesPresent(expectedCurrentMedicinesData)
    }
    @Then("^I see the expected discontinued medicines - Medical Record v2$")
    fun thenISeeExpectedDiscontinuedMedicinesV2() {
        assertExpectedMedicinesPresent(expectedDiscontinuedMedicinesData)
    }

    @When("^The GP practice responds with bad medications data")
    fun theGPPracticeRespondsWithBadData() {
        MedicationsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                .respondWithBadData(SerenityHelpers.getPatient())
    }

    private fun assertExpectedMedicinesPresent(expectedData: Map<Supplier, Array<String>>){
        val medicinesMessages = medicinesDetailPage.getMedicinesElements()

        val supplier = SerenityHelpers.getGpSupplier()

        Assert.assertEquals(
                "Expected records", expectedData[supplier]?.size, medicinesMessages.size )
        medicinesMessages.forEachIndexed { i, message ->
            Assert.assertEquals(expectedData[supplier]?.get(i), message.text) }

    }

    private fun formatDate(dateString: String) : String{
        val localDateTime = LocalDateTime.parse(dateString)
        val formatter = DateTimeFormatter.ofPattern("d MMMM uuuu")
        return formatter.format(localDateTime.toLocalDate())
    }

}
