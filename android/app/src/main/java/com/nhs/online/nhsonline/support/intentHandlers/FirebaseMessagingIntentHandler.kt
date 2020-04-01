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

class FirebaseMessagingIntentHandler(private val context: Context) : IIntentHandler
{
    override val intentAction = "firebaseMessaging"

    override fun handle(intent: Intent, isAppClosed: Boolean, nhsWeb: NhsWeb) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering Firebase Messaging handle")
        urlHelper = UrlHelper(context)
        appPersistData = PersistData(context)

        intent.extras?.get("url")?.let { url ->
            val urlString = UrlHelper(context).ensureUrlWithScheme(url.toString())
            if (urlHelper.isSameHostAndSchemeAsHomeUrl(url.toString())) {
                if (isAppClosed) {
                    urlString?.let { appPersistData.storePersistedLink(it.toString()) }
                } else {
                    urlString?.let { nhsWeb.loadUrl(it.toString()) }
                }
            }
        }
    }
}