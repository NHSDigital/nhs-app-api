package com.nhs.online.nhsonline.support

import android.graphics.Typeface.BOLD
import android.text.SpannableStringBuilder
import android.text.style.StyleSpan
import android.widget.TextView


fun TextView.setServiceError(header: String, info: String? = null, moreInfo: String? = null) {
    val builder = SpannableStringBuilder()
    builder.appendText(header, 0, true)
    info?.let { builder.appendText(it) }
    moreInfo?.let { builder.appendText(it, 2) }

    this.text = builder
}

fun SpannableStringBuilder.appendText(
    text: String,
    beforeNewLines: Int = 1,
    isHeader: Boolean = false
): SpannableStringBuilder {
    if (beforeNewLines > 0)
        this.append(beforeNewLines.toStringNewLines())

    val start = this.length
    this.append(text)
    if (isHeader) {
        val boldSpan = StyleSpan(BOLD)
        this.setSpan(boldSpan, start, this.length, 0)
    }
    return this
}

fun Int.toStringNewLines(): String {
    var lines = ""
    for (i in 0 until this) lines += "\n"
    return lines
}

