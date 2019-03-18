package mocking.organDonation.models

class OrganDonationDemographics(
        val faithDeclaration: FaithDeclaration = FaithDeclaration.None,
        val religion : KeyValuePair<String, String> = KeyValuePair("10", "Christian - Catholic"),
        val ethnicity: KeyValuePair<String, String> = KeyValuePair("1", "White - British")
)

class KeyValuePair<TKey, TValue>(val key:TKey, val value:TValue){
    override fun toString():String{
        return "Key: '$key', Value: '$value'"
    }
}
