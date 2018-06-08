package com.nhs.online.nhsonline.interfaces


interface IInteractor {

    fun showProgressDialog()

    fun dismissProgressDialog()

    fun selectSymptomsMenuActive()

    fun selectMoreMenuActive()

    fun showUnavailabilityError(unavailabilityErrorMessage: String?)

    fun showWebviewScreen()

    fun setHeaderText(text: String)

    fun clearMenuBarItem()

    fun hideHeader()

    fun hideMenuBar()
}