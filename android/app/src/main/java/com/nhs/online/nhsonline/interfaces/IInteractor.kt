package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage


interface IInteractor {

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun selectSymptomsMenuActive()

    fun selectMoreMenuActive()

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setHeaderText(text: String)

    fun clearMenuBarItem()

    fun hideHeader()

    fun hideMenuBar()
}