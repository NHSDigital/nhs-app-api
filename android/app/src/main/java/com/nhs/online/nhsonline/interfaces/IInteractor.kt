package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage

interface IInteractor {

    fun loadPage(url: String)

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun showBlankScreen()

    fun hideBlankScreen()

    fun dismissSessionExtensionDialog()

    fun selectNavigationMenuActive(navigationMenuId: Int)

    fun showUnavailabilityError(errorMessage: ErrorMessage)

    fun showWebviewScreen()

    fun setWebViewVisible()

    fun setHeaderText(text: String, description: String? = null)

    fun clearMenuBarItem()

    fun showHeader()

    fun showHeaderSlim()

    fun showMenuBar()

    fun setMenuBarItem(index: Int)

    fun hideHeader()

    fun hideHeaderSlim()

    fun hideMenuBar()

    fun announcePageTitle(title: String?)

    fun showExtendSessionDialogue(sessionDuration: Int)

    fun showBiometricLoginIfEnabled(): Boolean

    fun showNativeBiometricOptions()

    fun resetFocusToNhsLogoForAccessibility()

    fun pageLoadComplete()

    fun displayBiometricLoginErrorOccurrence()

    fun canDisplayBiometricLogin(): Boolean
}