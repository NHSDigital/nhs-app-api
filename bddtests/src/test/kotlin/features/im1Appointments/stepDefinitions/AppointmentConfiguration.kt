package features.im1Appointments.stepDefinitions

import mocking.emis.practices.NecessityOption
import org.junit.Assert

class AppointmentConfiguration {
    var firstNumber: String = ""
    var secondNumber: String = ""
    var reasonNecessity: NecessityOption = NecessityOption.OPTIONAL
    var inputType: TelephoneInputType = TelephoneInputType.Select
    var enterSymptoms: Boolean = false

    companion object {
        private var numbersKey = "number of stored telephone numbers"
        private var reasonNecessityKey = "the reason on the appointment is"
        private var inputTypeKey = "selecting telephone number"
        private var symptomsToEnter = "symptoms to enter"

        fun fromMappings(mappings: Map<String, String>): AppointmentConfiguration {
            val config = AppointmentConfiguration()

            if (mappings.containsKey(numbersKey)) {
                val myNumbers = mappings[numbersKey]!!.toInt()
                if (myNumbers == 1) {
                    config.firstNumber = "01234 456789"
                }
                if (myNumbers == 2) {
                    config.firstNumber = "01234 456789"
                    config.secondNumber = "01234 456780"
                }
                if (myNumbers > 2) {
                    Assert.fail("Cannot create a patient with more than two numbers")
                }
            }
            if (mappings.containsKey(reasonNecessityKey)) {
                config.reasonNecessity = reasonMapper(mappings[reasonNecessityKey]!!)
            }
            if (mappings.containsKey(inputTypeKey)) {
                config.inputType = inputMapper(mappings[inputTypeKey]!!)
            }
            if (mappings.containsKey(symptomsToEnter)) {
                config.enterSymptoms = mappings[symptomsToEnter]!! == "yes"
            }
            return config
        }

        private fun reasonMapper(reason: String): NecessityOption {
            val map = mapOf("mandatory" to NecessityOption.MANDATORY,
                    "optional" to NecessityOption.OPTIONAL,
                    "not allowed" to NecessityOption.NOT_ALLOWED)
            if (!map.containsKey(reason)) {
                Assert.fail("reason not in map")
            }
            return map[reason.toLowerCase()]!!
        }

        private fun inputMapper(reason: String): TelephoneInputType {
            val map = mapOf("select" to TelephoneInputType.Select,
                    "manual" to TelephoneInputType.Manual)
            if (!map.containsKey(reason)) {
                Assert.fail("reason not in map")
            }
            return map[reason.toLowerCase()]!!
        }
    }
}

enum class TelephoneInputType {
    Select,
    Manual
}