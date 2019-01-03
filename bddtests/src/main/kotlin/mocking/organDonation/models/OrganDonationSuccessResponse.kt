package mocking.organDonation.models

data class OrganDonationSuccessResponse<TBody>(
        var entry: List<TBody>)
