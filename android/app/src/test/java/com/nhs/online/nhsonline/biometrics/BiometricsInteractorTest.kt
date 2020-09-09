package com.nhs.online.nhsonline.biometrics

import android.content.res.Resources
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.support.LifeCycleObserverContext
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class BiometricsInteractorTest {

    private lateinit var mockInteractor: IInteractor
    private lateinit var mockNhsWeb: NhsWeb
    private lateinit var biometricInteractor: BiometricsInteractor

    @Before
    fun setUp() {

        val mockResources: Resources = mock {
            on { getString(any())}.thenReturn("myString") }

        val mockLifeCycleObserverContext: LifeCycleObserverContext = mock()

        mockNhsWeb = mock()
        mockInteractor = mock()

        biometricInteractor = BiometricsInteractor(mockInteractor, mockNhsWeb, mockLifeCycleObserverContext)
    }

    @Test
    fun test_dismissBiometricNotification() {
        //Act
        biometricInteractor.dismissBiometricNotification()

        //Assert
        verify(mockNhsWeb).onBiometricOptionChanged()
    }

    @Test
    fun test_dismissProgressDialog() {
        //Act
        biometricInteractor.dismissProgressDialog()

        //Assert
        verify(mockInteractor).dismissProgressDialog()
    }

    @Test
    fun test_showProgressDialog() {
        //Act
        biometricInteractor.showProgressDialog()

        //Assert
        verify(mockInteractor).showProgressDialog()
    }

    @Test
    fun test_getActivity() {
        //Act
        val result = biometricInteractor.getActivity()

        //Assert
        verify(mockInteractor).getActivity()
    }

}
