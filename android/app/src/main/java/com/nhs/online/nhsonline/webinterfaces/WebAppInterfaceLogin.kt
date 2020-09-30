package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.data.PaycassoData
import com.nhs.online.nhsonline.data.PaycassoDocumentTypeAdapter
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import com.squareup.moshi.JsonAdapter
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory

class WebAppInterfaceNhsLogin(
    private val activity: Activity,
    private val nhsWeb: NhsWeb
) {
    @JavascriptInterface
    fun startPaycasso(paycassoPayload: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering startPaycasso")
        val paycassoData = moshiBuilder
            .fromJson(paycassoPayload) ?: return
        runAction { nhsWeb.startPaycasso(paycassoData) }
    }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.NhsLogin){
            activity.runOnUiThread(action)
        }
    }

    private val moshiBuilder: JsonAdapter<PaycassoData>
        get() {
            return Moshi.Builder()
                .add(KotlinJsonAdapterFactory())
                .add(PaycassoDocumentTypeAdapter(nhsWeb.onFailure))
                .build()
                .adapter(PaycassoData::class.java)
        }
}
