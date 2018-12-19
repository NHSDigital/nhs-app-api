package com.nhs.online.nhsonline.utils

import android.os.Build
import android.text.Html
import android.text.Spanned

class Html {
    companion object {
        @Suppress("DEPRECATION")
        fun fromHtml(html:String): Spanned {
            if ( Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                return Html.fromHtml(html, Html.FROM_HTML_MODE_LEGACY)
            }
            else {
                return Html.fromHtml(html)
            }
        }
    }
}