package com.nhs.online.nhsonline.services

import com.google.android.gms.tasks.Task
import com.google.android.gms.tasks.Tasks
import com.google.firebase.iid.InstanceIdResult
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhaarman.mockito_kotlin.whenever
import com.nhs.online.nhsonline.clients.FirebaseClient
import com.nhs.online.nhsonline.utils.NotificationManagerCompat
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class NotificationsServiceTest {

    private lateinit var notificationsService: NotificationsService
    private lateinit var firebaseClient: FirebaseClient
    private lateinit var appWebInterface: AppWebInterface
    private lateinit var notificationManager: NotificationManagerCompat

    @Before
    fun setUp() {
        appWebInterface = mock()
        firebaseClient = mock()
        notificationManager = mock()
        notificationsService = NotificationsService(appWebInterface, firebaseClient, notificationManager)
    }

    @Test
    fun registerForPushNotifications_whenItGetsToken_shouldCallAuthorised() {
        val task: Task<InstanceIdResult> = Tasks.forResult(mock { on { token }.thenReturn("12345") })
        whenever(firebaseClient.instanceId).thenReturn(task)
        whenever(notificationManager.areNotificationsEnabled()).thenReturn(true)

        notificationsService.registerForPushNotifications("load")

        verify(appWebInterface).notificationsAuthorised("12345", "load")
    }

    @Test
    fun registerForPushNotifications_whenGettingTokenThrowsException_shouldCallUnauthorised() {
        whenever(firebaseClient.instanceId).thenThrow(IllegalStateException())
        whenever(notificationManager.areNotificationsEnabled()).thenReturn(true)

        notificationsService.registerForPushNotifications("load")

        verify(appWebInterface).notificationsUnauthorised()
    }

    @Test
    fun registerForPushNotifications_whenNotificationsNotEnabled_shouldCallUnauthorised() {
        whenever(notificationManager.areNotificationsEnabled()).thenReturn(false)

        notificationsService.registerForPushNotifications("load")

        verify(appWebInterface).notificationsUnauthorised()
    }

    @Test
    fun getNotificationsStatus_whenNotificationsAreEnabled_shouldCallbackWebWithAuthorised() {
        whenever(notificationManager.areNotificationsEnabled()).thenReturn(true)

        notificationsService.getNotificationsStatus()

        verify(appWebInterface).getNotificationsStatus("authorised")
    }

    @Test
    fun getNotificationsStatus_whenNotificationsAreNotEnabled_shouldCallbackWebWithDenied() {
        whenever(notificationManager.areNotificationsEnabled()).thenReturn(false)

        notificationsService.getNotificationsStatus()

        verify(appWebInterface).getNotificationsStatus("denied")
    }
}