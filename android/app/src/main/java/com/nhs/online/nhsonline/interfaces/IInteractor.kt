package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage


interface IInteractor {

    fun loadPage(url: String)

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun selectNavigationMenuActive(navigationMenuId: Int)

    fun goToCheckSymptoms()

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setHeaderText(text: String)

    fun clearMenuBarItem()

    fun setReloadUrl(url: String?)
}