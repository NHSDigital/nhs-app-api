package mocking.defaults.dataPopulation.journies.termsAndConditions

import mocking.sharedModels.ConsentMongoSerenityHelpers
import mocking.termsAndConditions.MongoTermsAndConditions
import models.Patient
import org.joda.time.DateTime
import utils.getOrNull
import utils.set
import utils.setIfNotAlreadySet

object TermsAndConditionsJourneyFactory {
    fun consent(patient: Patient, consentDate: DateTime = DateTime.now()) {
        val consentConfigured = ConsentMongoSerenityHelpers
                .CONSENT_DATA_CONFIGURED
                .getOrNull<Boolean>()

        if (consentConfigured == null || !consentConfigured) {
            MongoTermsAndConditions.addTermsAndConditionsAcceptance(patient.subject, consentDate)
            ConsentMongoSerenityHelpers
                    .CONSENT_DATA_CONFIGURED
                    .set(true)
        }
    }

    fun removeConsents() {
        MongoTermsAndConditions.clearTermsAndConditionsAcceptance()
        ConsentMongoSerenityHelpers
                .CONSENT_DATA_CONFIGURED
                .setIfNotAlreadySet(true)
    }
}