package mocking.data.prescriptions

import constants.Supplier
import models.prescriptions.PrescriptionLoaderConfiguration
import java.util.*

interface IPrescriptionLoader<T> {
    var data: T

    fun loadData(prescriptionLoaderConfig: PrescriptionLoaderConfiguration,
                 prescriptionCompletedByProxy: Boolean = false)

    fun getCourseName(): String {
        return getStringValue(getMedicationCourseNames())
    }

    fun getDosage(): String {
        return getStringValue(getDosages())
    }

    private fun getStringValue(list: List<String>): String {
        return list[getRandomNumber(getMedicationCourseNames().size)]
    }

    private fun getMedicationCourseNames(): List<String> {
        return listOf(
                "Ranitidine 150mg effervescent tablets",
                "Codine 200mg tablets",
                "Choline salicylate 8.7% oromucosal gel sugar free",
                "Paracetamol 150mg oral tablets",
                "Penicillin 150mg oral tablets"
        )
    }

    private fun getDosages(): List<String> {
        return listOf(
                "One To Be Taken Twice A Day",
                "One To Be Taken Three Times A Day",
                "One To Be Taken Weekly",
                "Two To Be Taken Four Times A Day",
                "One To Be Take Every Evening"
        )
    }

    fun getQuantity(quantity: Int): String {
        val list = listOf(
                "$quantity gram",
                "$quantity tablet",
                "$quantity ml")

        return list.get(getRandomNumber(list.size))
    }

    fun getRandomNumber(maxNum: Int): Int {
        val random = Random()
        val minNum = 1
        var localMaxNum = maxNum

        if (localMaxNum == 1) {
            localMaxNum += 1
        }

        return random.nextInt(localMaxNum - minNum) + minNum
    }

    companion object {

        fun getPrescriptionsLoader(gpSystem: Supplier): IPrescriptionLoader<*> {
            return when (gpSystem) {
                Supplier.EMIS -> {
                    EmisPrescriptionLoader
                }
                Supplier.TPP -> {
                    TppPrescriptionLoader
                }
                Supplier.VISION -> {
                    VisionPrescriptionLoader
                }
                Supplier.MICROTEST -> {
                    MicrotestPrescriptionLoader
                }
            }
        }
    }
}
