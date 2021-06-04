package features.serviceJourneyRules.mappers

import features.serviceJourneyRules.factories.SJRJourneyType

class SJRJourneyTypesMapper {
    companion object {
        private val enabledDefinitions = hashMapOf(
            "p5 vaccine records from net company" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_P5_ENABLED
            ),
            "vaccine records from net company" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_ENABLED
            ),
            "vaccine records from nhsd" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NHSD_ENABLED
            )
        )

        private val disabledDefinitions = hashMapOf(
            "111" to arrayListOf(
                SJRJourneyType.ONE_ONE_ONE_DISABLED
            ),
            "coronavirus information" to arrayListOf(
                SJRJourneyType.CORONAVIRUS_INFORMATION_DISABLED
            ),
            "p5 vaccine records from net company" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_P5_DISABLED
            ),
            "vaccine records from net company" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_DISABLED
            ),
            "vaccine records from nhsd" to arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NHSD_DISABLED
            )
        )

        fun map(feature: String, disabled: Boolean = false): Collection<SJRJourneyType> {
            val definitions = if (disabled) disabledDefinitions else enabledDefinitions
            return  definitions[feature.toLowerCase()]!!
        }
    }
}
