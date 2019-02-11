package mocking.organDonation

import mocking.organDonation.models.Issue

data class OrganDonationRegistrationResponse(var id: String, var issue: Issue? = null)
