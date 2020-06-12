package com.nhs.online.nhsonline.interfaces

import android.support.v4.app.FragmentActivity
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

    fun clearMenuBarItem()

    fun showHeader()

    fun showHeaderSlim()

    fun showMenuBar()

    fun setZoomable(canZoom: Boolean)

    fun setHelpUrl(url: String)

    fun setRetryPath(url: String)

    fun setMenuBarItem(index: Int)

    fun hideHeader()

    fun hideHeaderSlim()

    fun hideMenuBar()

    fun startDownload(base64Data: String, fileName: String, mimeType: String)

    fun announcePageTitle(title: String?)

    fun showExtendSessionDialogue()

    fun showBiometricLoginIfEnabled(forceStart: Boolean = false): Boolean

    fun dismissBiometricDialog()

    fun fetchBiometricSpec()

    fun resetFocusToNhsLogoForAccessibility()

    fun pageLoadComplete()

    fun displayBiometricLoginErrorOccurrence()

    fun canDisplayBiometricLogin(): Boolean

    fun dismissSplashScreen()

    fun getActivity(): FragmentActivity

    fun updateBiometricRegistration()

}
