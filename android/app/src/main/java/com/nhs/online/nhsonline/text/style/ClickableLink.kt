package com.nhs.online.nhsonline.text.style

import android.text.Selection
import android.text.Spannable
import android.text.TextPaint
import android.text.style.ClickableSpan
import android.view.View
import android.widget.TextView

class ClickableLink(private val onClickListener: View.OnClickListener): ClickableSpan() {
    override fun updateDrawState(textPaint: TextPaint) {
        textPaint.color = textPaint.linkColor
        textPaint.isUnderlineText = true
    }

    override fun onClick(view: View) {
        Selection.setSelection((view as TextView).text as Spannable, 0)
        view.invalidate()
        onClickListener.onClick(view)
    }
}
