package com.nhs.online.nhsonline.support.uiinteraction

import android.util.Log
import com.nhs.online.nhsonline.activities.HeaderIcon
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel

private val TAG = LoggedInHeaderStrategy::class.java.simpleName

class LoggedOutHeaderStrategy(private val uiInteractor: IInteractor): IHeaderStrategy {
    override fun apply(integrationLevel: IntegrationLevel) {
        when (integrationLevel) {
            IntegrationLevel.Gold -> {
                uiInteractor.hideHeader()
                uiInteractor.hideMenuBar()
            }
            IntegrationLevel.GoldWithNoHeaders -> uiInteractor.hideHeaderAndMenu()
            IntegrationLevel.SilverWithWebNavigation, IntegrationLevel.GoldOverlay -> uiInteractor.showHeaderSlim(HeaderIcon.Back)
            IntegrationLevel.SilverWithoutWebNavigation -> uiInteractor.showHeaderSlim(HeaderIcon.Close)
            else -> {
                Log.e(TAG, "Invalid Webview integration level: ${integrationLevel}")
                uiInteractor.hideHeaderAndMenu()
            }
        }
    }
}

