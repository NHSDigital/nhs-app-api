package com.nhs.online.nhsonline.support

import android.os.Build
import com.nhs.online.nhsonline.web.UserAgentBuilder
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class UserAgentBuilderTests {
    @Test
    fun buildUserAgentString_returnsFullUserAgentString() {
        val userAgent = "test-user-agent"
        val actualFullUserAgent = UserAgentBuilder.buildUserAgentString(userAgent)
        val expectedFullUserAgent = userAgent +
                " nhsapp-android/${com.nhs.online.nhsonline.BuildConfig.VERSION_NAME}" +
                " nhsapp-manufacturer/${Build.MANUFACTURER}" +
                " nhsapp-model/${Build.MODEL}" +
                " nhsapp-os/${Build.VERSION.RELEASE}" +
                " nhsapp-architecture/${Build.SUPPORTED_ABIS.joinToString(",") }"

        Assert.assertEquals(expectedFullUserAgent, actualFullUserAgent)
    }
}
