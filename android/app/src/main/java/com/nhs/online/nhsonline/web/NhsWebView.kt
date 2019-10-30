package com.nhs.online.nhsonline.web

import android.content.Context
import android.util.AttributeSet
import android.view.MotionEvent
import android.webkit.WebView
import com.nhs.online.nhsonline.activities.MainActivity

 class NhsWebView : WebView {
     constructor(context: Context) : super(context)
     constructor(context: Context, attrs: AttributeSet) : super(context, attrs)

     constructor(context: Context, attrs: AttributeSet, defStyleAttr: Int = 0) : super(context, attrs, defStyleAttr)

     override fun onFilterTouchEventForSecurity(event: MotionEvent?): Boolean {
         if(event != null && event.flags and MotionEvent.FLAG_WINDOW_IS_OBSCURED == MotionEvent.FLAG_WINDOW_IS_OBSCURED)
        {
            val activity = context as MainActivity
            activity.showOverlayDetectedDialog()
            return false
        }
         return super.onFilterTouchEventForSecurity(event)
     }
}