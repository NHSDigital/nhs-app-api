package com.nhs.online.nhsonline.support

import android.graphics.Typeface.BOLD
import android.text.SpannableStringBuilder
import android.text.style.StyleSpan
import android.widget.TextView
import org.json.JSONException
import org.json.JSONObject


fun TextView.setServiceError(header: String, message: String? = null) {
    val builder = SpannableStringBuilder()
    builder.appendText(header, 0)
    message?.let { builder.appendText(it) }

    this.text = builder
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

fun String.extractJSONString(key: String): String {
    return try {
        JSONObject(this).getString(key)
            .replace("\\\"", "\"")
    } catch (e: JSONException) {
        this
    }
}
