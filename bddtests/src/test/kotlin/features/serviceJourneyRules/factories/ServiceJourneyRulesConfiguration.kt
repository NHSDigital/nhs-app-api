package features.serviceJourneyRules.factories

import org.junit.Assert

class ServiceJourneyRulesConfiguration(val journey: String, val value: String) {

    fun toJourneyType(): SJRJourneyType {
        val journeyType = "${journey}_$value".replace(" ", "_").toUpperCase()

        Assert.assertTrue("Test setup incorrect, journey `$journey` does not contain value for `$value`",
                enumValues<SJRJourneyType>().any {
                    it.name == journeyType })

        return SJRJourneyType.valueOf(journeyType)
    }
}
