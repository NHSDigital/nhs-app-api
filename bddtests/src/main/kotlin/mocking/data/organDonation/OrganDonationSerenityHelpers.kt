package mocking.data.organDonation

import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers

enum class OrganDonationSerenityHelpers {
    DEMOGRAPHICS,
    EXPECTED_REGISTRATION_ID,
    IS_AMEND_JOURNEY,
    ORGAN_DONATION_DECISION,
    ORGAN_DONATION_WITHDRAWAL,
    ORGAN_DONATION_WITHDRAWAL_REASON,
    SOME_ORGANS_EXISTING,
    SOME_ORGANS_UPDATED,
    REFERENCE_ETHNICITIES,
    REFERENCE_RELIGIONS,
    REFERENCE_WITHDRAWAL_REASONS
}

fun <T>OrganDonationSerenityHelpers.getOrFail() : T {
    Assert.assertTrue("Test setup incorrect, $this to be set",
            Serenity.hasASessionVariableCalled(this))
    return Serenity.sessionVariableCalled<T>(this)
}

fun <T>OrganDonationSerenityHelpers.getOrNull() : T? {
    return SerenityHelpers.getValueOrNull(this)
}

fun <T>OrganDonationSerenityHelpers.set(value : T) {
    Serenity.setSessionVariable(this).to(value)
}

fun OrganDonationSerenityHelpers.isTrueOrFalse() : Boolean {
    return this.getOrNull<Boolean>() == true
}
