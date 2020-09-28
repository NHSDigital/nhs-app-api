package com.nhs.online.nhsonline.support

import android.text.Spanned
import android.text.SpannableString
import android.text.SpannableStringBuilder
import android.text.method.LinkMovementMethod
import android.view.View
import android.widget.TextView
import com.nhs.online.nhsonline.text.style.ClickableLink

fun TextView.setServiceError(header: String, message: String? = null) {
    val builder = SpannableStringBuilder()
    if (header.isNotEmpty()){
        builder.appendText(header, 0)
        message?.let { builder.appendText(it) }
    } else {
        builder.appendText(message!!, 0)
    }
    this.text = builder
}

fun TextView.makeLinks(vararg links: Pair<String, View.OnClickListener>) {
    if (links.isEmpty()) {
        return
    }

    var hasMatch = false
    val currentText = SpannableString(this.text)

    for (link in links) {
        val startIndex = this.text.toString().indexOf(link.first)
        val endIndex = startIndex + link.first.length

        if (startIndex >= 0) {
            hasMatch = true
            currentText.setSpan(ClickableLink(link.second), startIndex, endIndex, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE)
        }
    }

    if (hasMatch) {
        this.movementMethod = LinkMovementMethod.getInstance()
        this.setText(currentText, TextView.BufferType.SPANNABLE)
    }
}

fun SpannableStringBuilder.appendText(
    text: String,
    beforeNewLines: Int = 1
): SpannableStringBuilder {
    if (beforeNewLines > 0)
        this.append(beforeNewLines.toStringNewLines())

    this.append(text)

    return this
}

fun Int.toStringNewLines(): String {
    var lines = ""
    for (i in 0 until this) lines += "\n"
    return lines
}

fun ByteArray.toBase64(): String {
    return android.util.Base64.encodeToString(this,
        android.util.Base64.NO_WRAP)
}
