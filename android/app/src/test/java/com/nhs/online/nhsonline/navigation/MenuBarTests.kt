package com.nhs.online.nhsonline.navigation

import android.app.Activity
import android.view.LayoutInflater
import android.view.ViewGroup
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.support.ApplicationState
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.After
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito.validateMockitoUsage
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.android.controller.ActivityController

@RunWith(RobolectricTestRunner::class)
class MenuBarTests  {
    private lateinit var menuBar: MenuBar
    private lateinit var nhsWeb: NhsWeb
    private lateinit var selector: ((menuBarItem: MenuBarItem) -> Unit)

    @Before
    fun setUp() {
        val controller: ActivityController<Activity> = Robolectric.buildActivity(Activity::class.java)

        val activityMain = LayoutInflater.from(controller.get()).inflate(R.layout.activity_main, null) as ViewGroup

        for(i in 0.until(activityMain.childCount)) {
            if(activityMain.getChildAt(i) is MenuBar) {
                menuBar = activityMain.getChildAt(i) as MenuBar
                break
            }
        }

        selector = mock {}
        menuBar.menuItemSelectedListener = selector
    }

    @Test
    fun switchingTheMenuItemShouldSetApplicationStateBusyIfTheMenuIsBlocking() {

        val appStateMock: ApplicationState = mock {
            on { isReady() } doReturn true
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb

        for(childIndex: Int in 0.until(menuBar.childCount)) {
            val menuBarItem = menuBar.getChildAt(childIndex) as MenuBarItem
            if(menuBarItem.isBlockingMenuItem()) {
                menuBar.switchActiveMenuItemTo(menuBarItem.id)
                break
            }
        }

        verify(appStateMock).block()
    }

    @Test
    fun switchingTheMenuItemShouldNotSetApplicationStateBusyIfTheMenuIsNonBlocking() {

        val appStateMock: ApplicationState = mock {
            on { isReady() } doReturn true
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb

        for(childIndex: Int in 0.until(menuBar.childCount)) {
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
    fun switchingTheMenuItemShouldNotSetApplicationStateBusyIfCurrentTabIsReselected() {

        val appStateMock: ApplicationState = mock {
            on { isReady() } doReturn true
        }

        nhsWeb = mock {
            on { applicationState } doReturn appStateMock
        }

        menuBar.nhsWeb = nhsWeb

        for(childIndex: Int in 0.until(menuBar.childCount)) {
            val menuBarItem = menuBar.getChildAt(childIndex) as MenuBarItem
            if(menuBarItem.isBlockingMenuItem() && !menuBarItem.isReselectableMenuItem()) {
                menuBar.switchActiveMenuItemTo(menuBarItem.id)
                break
            }
        }

        verify(appStateMock).isReady()
        verify(appStateMock).block()

        for(childIndex: Int in 0.until(menuBar.childCount)) {
            val menuBarItem = menuBar.getChildAt(childIndex) as MenuBarItem
            if(menuBarItem.isBlockingMenuItem() && !menuBarItem.isReselectableMenuItem()) {
                menuBar.switchActiveMenuItemTo(menuBarItem.id)
                break
            }
        }

        verify(appStateMock, times(2)).isReady()
        verify(appStateMock, times(1)).block()
        verifyNoMoreInteractions(appStateMock)
    }

    @Test
    fun switchingTheMenuItemWhenTheApplicationStateIsBusyShouldNotCallTheListener() {

        val appStateMock: ApplicationState = mock {
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