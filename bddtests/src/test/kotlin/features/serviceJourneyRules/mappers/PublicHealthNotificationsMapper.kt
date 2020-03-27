package features.serviceJourneyRules.mappers

import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.serviceJourneyRules.stepDefinitions.ServiceJourneyRulesSerenityHelpers
import utils.setIfNotAlreadySet
import worker.models.serviceJourneyRules.PublicHealthNotification
import worker.models.serviceJourneyRules.PublicHealthNotificationType
import worker.models.serviceJourneyRules.PublicHealthNotificationUrgency

open class PublicHealthNotificationsMapper {
    companion object{
        private val coronavirus_covid19 = PublicHealthNotification(
                "coronavirus_covid19",
                PublicHealthNotificationType.callout,
                PublicHealthNotificationUrgency.warning,
                "Coronavirus (COVID-19)",
                "<p class=\"nhsuk-u-margin-bottom-2\"><a href=\"https://nhs.uk/conditions/coronavirus-covid-19/" +
                        "\" target=\"_blank\">Get information about coronavirus on NHS.UK</a></p>"
        )

        fun setSerenityVariablesForJourneys(odsCode: String, configurations: List<ServiceJourneyRulesConfiguration>) {
            configurations.forEach {
                configuration ->
                when (Pair(odsCode, configuration.toJourneyType())) {
                    Pair(ServiceJourneyRulesMapper.Companion.ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION,
                         ServiceJourneyRulesMapper.Companion.JourneyType.HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS) -> {
                        ServiceJourneyRulesSerenityHelpers.HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS
                            .setIfNotAlreadySet(mutableListOf(coronavirus_covid19))
                    }
                }
            }
        }
    }
}