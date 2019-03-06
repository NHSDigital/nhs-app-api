package pages.organDonation

import pages.ErrorPage

class OrganDonationErrorPage: ErrorPage() {

    val errorHeader = "Something went wrong"
    val errorMessageWithRetry = "If the problem persists you can contact NHS Blood and Transplant " +
            "to get help with this."
    val errorMessageNoRetry = "You need to contact NHS Blood and Transplant to get help with this."
}
