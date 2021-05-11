package features.serviceJourneyRules.mappers

import features.serviceJourneyRules.factories.SJRJourneyType

class SJRJourneyTypesMapper {
    companion object {
        private val enabledDefinitions = hashMapOf<String, Collection<SJRJourneyType>>()

        private val disabledDefinitions = hashMapOf(
            "111" to arrayListOf(
                SJRJourneyType.ONE_ONE_ONE_DISABLED
            ),
            "coronavirus information" to arrayListOf(
                SJRJourneyType.CORONAVIRUS_INFORMATION_DISABLED
            )
        )

        fun map(feature: String, disabled: Boolean = false): Collection<SJRJourneyType> {
            val definitions = if (disabled) disabledDefinitions else enabledDefinitions
            return  definitions[feature.toLowerCase()]!!
        }
    }
}
