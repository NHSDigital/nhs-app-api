package com.nhs.online.nhsonline.web

import android.content.Context
import android.util.AttributeSet
import android.webkit.WebView

 class NhsWebView : WebView {
     constructor(context: Context) : super(context)
     constructor(context: Context, attrs: AttributeSet) : super(context, attrs)

     constructor(context: Context, attrs: AttributeSet, defStyleAttr: Int = 0) : super(context, attrs, defStyleAttr)

}