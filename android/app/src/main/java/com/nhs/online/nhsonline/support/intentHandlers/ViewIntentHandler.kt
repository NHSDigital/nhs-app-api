package com.nhs.online.nhsonline.support.intentHandlers

import android.content.Context
import android.content.Intent
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.web.NhsWeb

private lateinit var urlHelper: UrlHelper
private lateinit var appPersistData: PersistData

class ViewIntentHandler(private val context: Context) : IIntentHandler
{
    override val intentAction = Intent.ACTION_VIEW

    override fun handle(intent: Intent, isAppClosed: Boolean, nhsWeb: NhsWeb) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering View Intent handle")
        urlHelper = UrlHelper(context)
        appPersistData = PersistData(context)

        intent.dataString.let { url ->
            val urlString = urlHelper.createRedirectToUrl(url)
            if (urlHelper.isSameHostAndSchemeAsHomeUrl(urlString.toString())) {
                if (isAppClosed) {
                    urlString?.let { appPersistData.storePersistedLink(urlString.toString()) }
                } else {
                    urlString?.let { nhsWeb.loadUrl(urlString.toString()) }
                }
            }
        }
    }
}