package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.graphics.Bitmap
import android.webkit.WebView
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.SymptomsActivity
import kotlinx.android.synthetic.main.check_my_symptoms_banner.*

class UnsecureWebClient(
        private val unsecureController: SymptomsActivity,
        private val context: Context
) : WebViewClient() {

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {

        val urlPath = context.resources.getString(R.string.baseURL)+context.resources.getString(R.string.checkYourSymptoms)+context.resources.getString(R.string.nhsOnlineRequiredQueries)
        when(url) {
            context.resources.getString(R.string.nhs111) -> unsecureController.setHeaderText(context.resources.getString(R.string.nhs_111_header))
            context.resources.getString(R.string.conditions) -> unsecureController.setHeaderText(context.resources.getString(R.string.conditions_header))
            urlPath -> unsecureController.setHeaderText(context.resources.getString(R.string.symptoms_header))
        }

        super.onPageFinished(view, url)
    }

}