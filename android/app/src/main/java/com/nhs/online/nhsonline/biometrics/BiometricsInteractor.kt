package com.nhs.online.nhsonline.biometrics

import androidx.fragment.app.FragmentActivity
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.support.LifeCycleObserverContext
import com.nhs.online.nhsonline.web.NhsWeb

class BiometricsInteractor(private val mainInteractor: IInteractor,
                           private val nhsWeb:NhsWeb,
                           private val lifeCycleObserverContext: LifeCycleObserverContext) : IBiometricsInteractor {

    fun dismissBiometricNotification() {
        nhsWeb.onBiometricOptionChanged()
    }

    override fun dismissProgressDialog() {
        mainInteractor.dismissProgressDialog()
    }

    override fun getActivity(): FragmentActivity = mainInteractor.getActivity()

    override fun loadBiometricLoginPage(url: String) {
        var contextUrl = lifeCycleObserverContext.url!!

        contextUrl += when(contextUrl.contains("?")) {
            true -> "&$url"
            false -> "?$url"
        }

        nhsWeb.requiresFullPageLoad = true
        nhsWeb.loadUrl(contextUrl)
    }

    override fun showProgressDialog() {
        mainInteractor.showProgressDialog()
    }
}
