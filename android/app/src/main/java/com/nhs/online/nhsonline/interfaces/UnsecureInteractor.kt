package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage


interface UnsecureInteractor {

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setHeaderText(text: String, description: String? = null)
}