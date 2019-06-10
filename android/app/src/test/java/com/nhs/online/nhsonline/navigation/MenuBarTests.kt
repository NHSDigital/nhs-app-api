package com.nhs.online.nhsonline.navigation

import android.app.Activity
import android.view.LayoutInflater
import android.view.ViewGroup
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.support.ApplicationState
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.navigation.MenuBar
import org.junit.After
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.android.controller.ActivityController
import org.mockito.Mockito.validateMockitoUsage

@RunWith(RobolectricTestRunner::class)
class MenuBarTests  {
    private lateinit var menuBar: MenuBar
    private lateinit var nhsWeb: NhsWeb
    private lateinit var selector: ((menuBarItem: MenuBarItem) -> Unit)

    @Before
    fun setUp() {
        var controller: ActivityController<Activity> = Robolectric.buildActivity(Activity::class.java)

        var activityMain = LayoutInflater.from(controller.get()).inflate(R.layout.activity_main, null) as ViewGroup

        for(i in 0..activityMain.childCount-1) {
            if(activityMain.getChildAt(i) is MenuBar) {
                menuBar = activityMain.getChildAt(i) as MenuBar
                break
            }
        }

        selector = mock {}
        menuBar.menuItemSelectedListener = selector
    }

    @Test
    fun SwitchingTheMenuItemShouldSetApplicationStateBusyIfTheMenuIsBlocking() {

        var appStateMock: ApplicationState = mock {
            on { isReady() } doReturn true
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb

        for(childIndex: Int in 0..menuBar.childCount - 1) {
            val menuBarItem = menuBar.getChildAt(childIndex) as MenuBarItem
            if(menuBarItem.isBlockingMenuItem()) {
                menuBar.switchActiveMenuItemTo(menuBarItem.id)
                break
            }
        }

        verify(appStateMock).block()
    }

    @Test
    fun SwitchingTheMenuItemShouldNotSetApplicationStateBusyIfTheMenuIsNonBlocking() {

        var appStateMock: ApplicationState = mock {
            on { isReady() } doReturn true
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb

        for(childIndex: Int in 0..menuBar.childCount - 1) {
            val menuBarItem = menuBar.getChildAt(childIndex) as MenuBarItem
            if(!menuBarItem.isBlockingMenuItem()) {
                menuBar.switchActiveMenuItemTo(menuBarItem.id)
                break
            }
        }

        verify(appStateMock).isReady()
        verifyNoMoreInteractions(appStateMock)
    }


    @Test
    fun SwitchingTheMenuItemWhenTheApplicationStateIsBusyShouldNotCallTheListener() {

        var appStateMock: ApplicationState = mock {
            on { isReady() } doReturn false
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb
        val menuBarItem = menuBar.getChildAt(0) as MenuBarItem
        menuBar.switchActiveMenuItemTo(menuBarItem.id)
        verifyZeroInteractions(selector)

    }

    @After
    fun validate() {
        validateMockitoUsage()
    }
}