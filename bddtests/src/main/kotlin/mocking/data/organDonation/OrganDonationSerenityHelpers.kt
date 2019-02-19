package mocking.data.organDonation

import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers

enum class OrganDonationSerenityHelpers{
    ORGAN_DONATION_DECISION,
    IS_AMEND_JOURNEY,
    EXPECTED_REGISTRATION_ID,
    DEMOGRAPHICS,
    IS_OPT_IN,
    SOME_ORGANS_UPDATED,
    SOME_ORGANS_EXISTING,
    REFERENCE_ETHNICITIES,
    REFERENCE_RELIGIONS
}

fun <T>OrganDonationSerenityHelpers.getOrFail() : T{
    Assert.assertTrue("Test setup incorrect, $this to be set",
            Serenity.hasASessionVariableCalled(this))
    return Serenity.sessionVariableCalled<T>(this)
}

fun <T>OrganDonationSerenityHelpers.getOrNull() : T?{
    return SerenityHelpers.getValueOrNull(this)
}

fun <T>OrganDonationSerenityHelpers.set(value : T){
    Serenity.setSessionVariable(this).to(value)
}
