package com.nhs.online.nhsonline.utils

import android.os.Build
import java.lang.reflect.Field
import java.lang.reflect.Modifier

class SdkVersionHelper {

    companion object {
        @Throws(Exception::class)
        fun setSdkVersion(newValue: Any) {

            val field = Build.VERSION::class.java.getField("SDK_INT")

            field.isAccessible = true

            val modifiersField = Field::class.java.getDeclaredField("modifiers")
            modifiersField.isAccessible = true
            modifiersField.setInt(field, field.modifiers and Modifier.FINAL.inv())

            field.set(null, newValue)
        }

        @Throws(Exception::class)
        fun getSdkVersion(): Any {
            return Build.VERSION::class.java.getField("SDK_INT").get(Int)
        }
    }
}