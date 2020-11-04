package com.nhs.online.nhsonline.utils

import java.io.PrintWriter
import java.io.StringWriter
import java.lang.Exception

object ErrorUtils {
    fun dumpStackTrace(exception: Exception?): String {
        StringWriter().use { sw ->
            exception?.printStackTrace(PrintWriter(sw))

            return sw.toString()
        }
    }
}
