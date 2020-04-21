package mocking.termsAndConditions

import mongodb.MongoDBConnection
import mongodb.MongoRepositoryTermsAndConditions
import org.joda.time.DateTime

object MongoTermsAndConditions
{
    fun addTermsAndConditionsAcceptance(subject: String, consentDate: DateTime)
    {
        val termsAndConditionRecord = MongoRepositoryTermsAndConditions (
                NhsLoginId = subject,
                ConsentGiven = true,
                AnalyticsCookieAccepted = true,
                DateOfConsent = consentDate.toString(),
                DateOfAnalyticsCookieToggle = consentDate.toString()
        )

        MongoDBConnection.TermsAndConditionsCollection.clearAndInsertValue(termsAndConditionRecord)
    }
}
