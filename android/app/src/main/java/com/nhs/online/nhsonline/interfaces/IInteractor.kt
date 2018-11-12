package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage


interface IInteractor {

    fun loadThrottlingCarousel()

    fun loadPage(url: String)

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun selectNavigationMenuActive(navigationMenuId: Int)

    fun goToCheckSymptoms()

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setHeaderText(text: String, description: String? = null)

    fun clearMenuBarItem()

    fun showHeader()

    fun showMenuBar()

    fun hideHeader()

    fun hideMenuBar()

    fun setReloadUrl(url: String?)

    fun announcePageTitle(title: String?)

    fun showExtendSessionDialogue(sessionDuration: Int)
}