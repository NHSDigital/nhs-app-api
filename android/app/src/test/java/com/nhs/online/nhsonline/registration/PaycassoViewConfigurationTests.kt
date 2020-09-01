package com.nhs.online.nhsonline.registration

import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.view.configuration.DocumentPreviewViewConfiguration
import com.paycasso.sdk.api.flow.view.configuration.DocumentViewConfiguration
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.PreviewConfigurationSide
import com.paycasso.view.DocumentPreviewViewFragment
import com.paycasso.view.FirstBackTransitionFragment
import com.paycasso.view.FirstFrontTransitionFragment
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class PaycassoViewConfigurationTests {
    private lateinit var paycassoViewConfiguration: PaycassoViewConfiguration

    @Test
    fun getViewConfigurationBuilder_ReturnsViewForDriversLicenceIdType() {
        val documentConfigurations = setUpDocumentConfiguration(PaycassoDocumentType.DriversLicence)
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            transactionType = "transactionType",
            deviceId = "appDeviceId",
            appUserId = "appUserId",
            hasNfcJourney = false,
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Both front and back on Id type
        val expectedDocumentPreviewViewFrontConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewFrontConfiguration.previewSide = PreviewConfigurationSide.FRONT
        expectedDocumentPreviewViewFrontConfiguration.screen = DocumentPreviewViewFragment()

        val expectedDocumentPreviewViewBackConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewBackConfiguration.previewSide = PreviewConfigurationSide.BACK
        expectedDocumentPreviewViewBackConfiguration.screen = DocumentPreviewViewFragment()

        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewFrontConfiguration,
            expectedDocumentPreviewViewBackConfiguration)

        val expectedDocumentFrontViewConfiguration = DocumentViewConfiguration()
        expectedDocumentFrontViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentFrontViewConfiguration.screen = FirstFrontTransitionFragment()

        val expectedDocumentBackViewConfiguration = DocumentViewConfiguration()
        expectedDocumentBackViewConfiguration.documentSide = DocumentConfigurationSide.BACK
        expectedDocumentBackViewConfiguration.screen = FirstBackTransitionFragment()

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentFrontViewConfiguration,
            expectedDocumentBackViewConfiguration
        )

        Assert.assertEquals(expectedDocumentPreviewConfigurationList.size,
            viewConfiguration.documentPreviewViewConfigurations[0]!!.size)
        Assert.assertEquals(expectedDocumentViewConfigurationList.size,
            viewConfiguration.documentViewConfigurations[0]!!.size)

        Assert.assertEquals(PreviewConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is DocumentPreviewViewFragment)

        Assert.assertEquals(PreviewConfigurationSide.BACK,
            viewConfiguration.documentPreviewViewConfigurations[0]!![1].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![1].screen
                is DocumentPreviewViewFragment)

        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is FirstFrontTransitionFragment)

        Assert.assertEquals(DocumentConfigurationSide.BACK,
            viewConfiguration.documentViewConfigurations[0]!![1].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![1].screen
                is FirstBackTransitionFragment)
    }

    @Test
    fun getViewConfigurationBuilder_ReturnsViewForPhotoIdType() {
        val documentConfigurations = setUpDocumentConfiguration(PaycassoDocumentType.PhotoId)
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            transactionType = "transactionType",
            deviceId = "appDeviceId",
            appUserId = "appUserId",
            hasNfcJourney = false,
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Only front on PhotoId type
        val expectedDocumentPreviewViewFrontConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewFrontConfiguration.previewSide = PreviewConfigurationSide.FRONT
        expectedDocumentPreviewViewFrontConfiguration.screen = DocumentPreviewViewFragment()

        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewFrontConfiguration)

        val expectedDocumentFrontViewConfiguration = DocumentViewConfiguration()
        expectedDocumentFrontViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentFrontViewConfiguration.screen = FirstFrontTransitionFragment()

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentFrontViewConfiguration)

        Assert.assertEquals(expectedDocumentPreviewConfigurationList.size,
            viewConfiguration.documentPreviewViewConfigurations[0]!!.size)
        Assert.assertEquals(expectedDocumentViewConfigurationList.size,
            viewConfiguration.documentViewConfigurations[0]!!.size)

        Assert.assertEquals(PreviewConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is DocumentPreviewViewFragment)

        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is FirstFrontTransitionFragment)
    }

    @Test
    fun getViewConfigurationBuilder_ReturnsViewForPassportDocumentType() {
        val documentConfigurations = setUpDocumentConfiguration(PaycassoDocumentType.Passport)
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            transactionType = "transactionType",
            deviceId = "appDeviceId",
            appUserId = "appUserId",
            hasNfcJourney = false,
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Only front on Passport type
        val expectedDocumentPreviewViewConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewConfiguration.previewSide = PreviewConfigurationSide.FRONT
        expectedDocumentPreviewViewConfiguration.screen = DocumentPreviewViewFragment()

        val expectedDocumentViewConfiguration = DocumentViewConfiguration()
        expectedDocumentViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentViewConfiguration.screen = FirstFrontTransitionFragment()


        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewConfiguration
        )

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentViewConfiguration
        )

        // Assert lists are the same size
        Assert.assertEquals(expectedDocumentPreviewConfigurationList.size,
            viewConfiguration.documentPreviewViewConfigurations[0]!!.size)
        Assert.assertEquals(expectedDocumentViewConfigurationList.size,
            viewConfiguration.documentViewConfigurations[0]!!.size)

        // Assert documentPreviewViewConfigurations fields
        Assert.assertEquals(PreviewConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is DocumentPreviewViewFragment)

        // Assert documentViewConfigurations fields
        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is FirstFrontTransitionFragment)

    }

    private fun setUpDocumentConfiguration(type: PaycassoDocumentType): ArrayList<DocumentConfiguration> {
        paycassoViewConfiguration = PaycassoViewConfiguration(type)
        return arrayListOf(DocumentConfiguration(
            type.mrzLocation,
            type.barcodeLocation,
            type.faceLocation,
            type.documentShape,
            type.isBothSides,
            type.eChipPresence,
            type.docCheck,
            type.documentName
        ))
    }
}
