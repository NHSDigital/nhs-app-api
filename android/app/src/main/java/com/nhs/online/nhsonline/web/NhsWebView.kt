package com.nhs.online.nhsonline.web

import android.content.Context
import android.util.AttributeSet
import android.view.MotionEvent
import android.webkit.WebView
import com.nhs.online.nhsonline.activities.MainActivity

class NhsWebView @JvmOverloads constructor(
        context: Context, attrs: AttributeSet? = null, defStyleAttr: Int = 0
) : WebView(context, attrs, defStyleAttr) {

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
