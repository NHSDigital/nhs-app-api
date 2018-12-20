package mocking.organDonation.models

data class OrganDonationSuccessResponse<TBody>(
        var entry: List<Entry<TBody>>)

data class Entry<TBody>(var resource : TBody)
