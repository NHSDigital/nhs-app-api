package com.nhs.online.nhsonline.utils

import android.text.Html
import android.text.Spanned

class Html {
    companion object {
        fun fromHtml(html:String): Spanned {
            return Html.fromHtml(html, Html.FROM_HTML_MODE_LEGACY)
        }
    }
}