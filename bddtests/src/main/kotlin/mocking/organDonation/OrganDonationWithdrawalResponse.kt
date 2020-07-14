package mocking.organDonation

import mocking.organDonation.models.Issue

data class OrganDonationWithdrawalResponse(var id: String, var issue: List<Issue>? = null)
