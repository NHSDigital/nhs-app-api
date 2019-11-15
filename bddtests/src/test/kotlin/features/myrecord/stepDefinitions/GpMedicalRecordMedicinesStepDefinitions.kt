package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.myrecord.MedicationsData
import org.junit.Assert
import pages.gpMedicalRecord.MedicinesDetailPage
import pages.gpMedicalRecord.MedicinesIndexPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

open class GpMedicalRecordMedicinesStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var medicinesIndexPage: MedicinesIndexPage
    lateinit var medicinesDetailPage: MedicinesDetailPage


    val expectedAcuteMedicinesData = mapOf(
        "EMIS" to arrayOf(
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nPenicillin\nOne to be taken four times a day\n" +
                        "28 Capsules",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen\nOne to be taken twice a day\n" +
                        "14 Capsules"
        ), "VISION" to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
        ), "TPP" to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nVentolin"
        ), "MICROTEST" to arrayOf(
                "27 March 2019\nMedication 3\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
        )
    )

    val expectedCurrentMedicinesData = mapOf(
        "EMIS" to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nAmoxycillin\nOne to be taken twice a day\n" +
                        "14 Capsules",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nInhaler Mix\nMegaMix, consisting of:\n" +
                        "Ventolin - 150ml\nSalbutanol - 200ml\n" +
                        "One to be taken once a day\n2 inhalers",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen Plus\nOne to be taken once a day\n" +
                        "7 Capsules"
        ), "VISION" to arrayOf(
                "8 October 2018\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
        ), "TPP" to arrayOf(
                "${formatDate(MedicationsData.tenMonthsAgo)}\nVentolin",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nSalbutamol",
                "${formatDate(MedicationsData.tenMonthsAgo)}\nCalpol"
        ), "MICROTEST" to arrayOf(
                "27 March 2019\nMedication 1\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
        )
    )

    val expectedDiscontinuedMedicinesData = mapOf(
            "EMIS" to arrayOf(
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nAmoxycillin\nOne to be taken twice a day\n" +
                    "14 Capsules\nEnded: ${formatDate(MedicationsData.tenMonthsAgo)}",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nIbuprofen\nOne to be taken once a day\n" +
                    "7 Capsules\nEnded: ${formatDate(MedicationsData.tenMonthsAgo)}"
            ), "VISION" to arrayOf(
                "8 October 2018\nPanadol ActiFast 500mg tablets " +
                    "(GlaxoSmithKline Consumer Healthcare)\n" +
                    "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\n45 tablet"
            ), "TPP" to arrayOf(
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nVentolin",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nSalbutamol",
                "${formatDate(MedicationsData.twentyMonthsAgo)}\nCalpol"
            ), "MICROTEST" to arrayOf(
                "27 March 2019\nMedication 2\nONE tablet every day\n60 tabs\nReason: Reason: high blood pressure"
            )
    )

    @When("^I click the Acute medicines link - GP Medical Record$")
    fun whenIClickTheAcuteMedicinesLinkGpMedicalRecord() {
        medicinesIndexPage.clickAcuteMedicinesLink()
    }

    @When("^I click the Current medicines link - GP Medical Record$")
    fun whenIClickTheCurrentMedicinesLinkGpMedicalRecord() {
        medicinesIndexPage.clickCurrentMedicinesLink()
    }

    @When("^I click the Discontinued medicines link - GP Medical Record$")
    fun whenIClickTheDiscontinuedMedicinesLinkGpMedicalRecord() {
        medicinesIndexPage.clickDiscontinuedMedicinesLink()
    }

    @Then("^I see the expected acute medicines - GP Medical Record$")
    fun thenISeeExpectedAcuteMedicinesGpMedicalRecord() {
        assertExpectedMedicinesPresent(expectedAcuteMedicinesData)
    }

    @Then("^I see the expected current medicines - GP Medical Record$")
    fun thenISeeExpectedCurrentMedicinesGpMedicalRecord() {
        assertExpectedMedicinesPresent(expectedCurrentMedicinesData)
    }
    @Then("^I see the expected discontinued medicines - GP Medical Record$")
    fun thenISeeExpectedDiscontinuedMedicinesGpMedicalRecord() {
        assertExpectedMedicinesPresent(expectedDiscontinuedMedicinesData)
    }

    private fun assertExpectedMedicinesPresent(expectedData: Map<String, Array<String>>){
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
        val formattedDate = formatter.format(localDateTime.toLocalDate())
        return formattedDate
    }

}