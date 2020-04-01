package com.nhs.online.nhsonline.support.intentHandlers

import android.content.Context
import android.content.Intent
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.biometrics.BiometricsInterface

private lateinit var urlHelper: UrlHelper
private lateinit var appPersistData: PersistData
private lateinit var biometricsInterface: BiometricsInterface

class FidoIntentHandler(private val context: Context) : IIntentHandler
{
    override val intentAction = "fido"

    override fun handle(intent: Intent, isAppClosed: Boolean, nhsWeb: NhsWeb) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering Fido Intent Handle")

        intent?.data?.let { uri ->
            val uriPath = uri.path ?: ""

            val hasFidoLoginError = uriPath.contains(context.getString(R.string.authRedirectPath)) &&
                    biometricsInterface.isFingerprintRegistered &&
                    uri.queryParameterNames.contains(context.getString(R.string.redirectErrorQueryParam))
            if (hasFidoLoginError) {
                Log.d(Application.TAG, "Fido login error response url: $uri")
                biometricsInterface.notifyLoginErrorOccurrence()
                nhsWeb.loadWelcomePage()
                return
            }

            val hasAppScheme = uri.scheme == context.getString(R.string.appScheme)
            val url = if (hasAppScheme) uri.buildUpon()
                    .scheme(context.getString(R.string.baseScheme)).toString()
            else uri.toString()

            if (hasAppScheme)
                nhsWeb.showBlankScreen()

            nhsWeb.loadUrl(url)
        }
    }
}