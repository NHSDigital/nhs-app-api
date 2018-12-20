package models

import mocking.organDonation.models.FaithDeclaration

class OrganDonationDemographics(
        val faithDeclaration: FaithDeclaration = FaithDeclaration.NotStated,
        val religion : KeyValuePair<String, String> = KeyValuePair("10", "Christian - Catholic"),
        val ethnicity: KeyValuePair<String, String> = KeyValuePair("1", "White - British")
)

class KeyValuePair<TKey, TValue>(val key:TKey, val value:TValue)