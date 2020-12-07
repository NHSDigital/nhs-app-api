package com.nhs.online.nhsonline.support.fileDownloading

import android.util.Base64
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode

class Base64File(
        val fileName: String,
        val fileMimeType: String,
        val encoding: String,
        val source: JavaScriptInteractionMode? = null
) {
    private val DATA_SCHEME = "data:"

    val dataMediaType: String
    val data: String
    val createdFromDataScheme: Boolean
        get() = this.dataMediaType != ""

    init {
        if (encoding.startsWith(DATA_SCHEME, true)) {
            val clobWithoutScheme = encoding.substring(DATA_SCHEME.length)
            // split to place file mime type into first element
            val clobSplitOnComma = clobWithoutScheme.split(',')
            // this should contain ';base64' at end, so strip that as well
            dataMediaType = clobSplitOnComma[0].split(';')[0]
            data = clobSplitOnComma[1]
        }
        else {
            dataMediaType = ""
            data = encoding
        }

    }

    fun decode(): ByteArray {
        return Base64.decode(data, Base64.DEFAULT)
    }
}

