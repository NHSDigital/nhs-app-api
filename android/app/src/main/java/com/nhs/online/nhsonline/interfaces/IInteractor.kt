package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage


interface IInteractor {

    fun loadPage(url: String)

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun selectSymptomsMenuActive()

    fun selectMoreMenuActive()

    fun goToCheckSymptoms()

    fun showSymptomsBanner()

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setHeaderText(text: String)

    fun clearMenuBarItem()

    fun hideHeader()

    fun hideMenuBar()

    fun setReloadUrl(url: String?)
}