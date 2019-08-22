package features.serviceJourneyRules.factories

import org.junit.Assert

class ServiceJourneyRulesConfiguration(val journey: String, val value: String) {

    fun toJourneyType(): ServiceJourneyRulesMapper.Companion.JourneyType {
        val journeyType = "${journey}_$value".replace(" ", "_").toUpperCase()

        Assert.assertTrue("Test setup incorrect, journey `$journey` does not contain value for `$value`",
                enumValues<ServiceJourneyRulesMapper.Companion.JourneyType>().any { it.name == journeyType })

        return ServiceJourneyRulesMapper.Companion.JourneyType.valueOf(journeyType)
    }
}