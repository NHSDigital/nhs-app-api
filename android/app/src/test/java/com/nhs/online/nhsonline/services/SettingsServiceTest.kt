package com.nhs.online.nhsonline.services

import android.content.Context
import android.content.Intent
import android.provider.Settings
import com.nhaarman.mockitokotlin2.*
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class SettingsServiceTest {
    private lateinit var context:Context
    private lateinit var settingsService: SettingsService

    @Before
    fun setUp() {
        context = mock()
        settingsService = SettingsService(context)
    }

    @Test
    fun openAppSettings_callsStartActivity_withANewIntent() {
        doNothing().whenever(context).startActivity(any())

        settingsService.openSettings()

        argumentCaptor<Intent>().apply {
            verify(context).startActivity(capture())
            Assert.assertEquals(Settings.ACTION_APPLICATION_DETAILS_SETTINGS, firstValue.action)
        }
    }
}