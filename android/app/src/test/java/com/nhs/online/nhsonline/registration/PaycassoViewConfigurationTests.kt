package com.nhs.online.nhsonline.registration

import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.view.configuration.DocumentPreviewViewConfiguration
import com.paycasso.sdk.api.flow.view.configuration.DocumentViewConfiguration
import com.paycasso.sdk.api.flow.view.configuration.enums.ConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.view.nhs.documentpreview.BackDocumentPreviewFragment
import com.paycasso.view.nhs.documentpreview.FrontDocumentPreviewFragment
import com.paycasso.view.nhs.transition.*
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
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Both front and back on Id type
        val expectedDocumentPreviewViewFrontConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewFrontConfiguration.previewSide = ConfigurationSide.FRONT
        expectedDocumentPreviewViewFrontConfiguration.screen = IdFrontTransitionFragment()

        val expectedDocumentPreviewViewBackConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewBackConfiguration.previewSide = ConfigurationSide.BACK
        expectedDocumentPreviewViewBackConfiguration.screen = IdBackTransitionFragment()

        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewFrontConfiguration,
            expectedDocumentPreviewViewBackConfiguration)

        val expectedDocumentFrontViewConfiguration = DocumentViewConfiguration()
        expectedDocumentFrontViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentFrontViewConfiguration.screen = DlFrontTransitionFragment()

        val expectedDocumentBackViewConfiguration = DocumentViewConfiguration()
        expectedDocumentBackViewConfiguration.documentSide = DocumentConfigurationSide.BACK
        expectedDocumentBackViewConfiguration.screen = DlBackTransitionFragment()

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentFrontViewConfiguration,
            expectedDocumentBackViewConfiguration
        )

        Assert.assertEquals(expectedDocumentPreviewConfigurationList.size,
            viewConfiguration.documentPreviewViewConfigurations[0]!!.size)
        Assert.assertEquals(expectedDocumentViewConfigurationList.size,
            viewConfiguration.documentViewConfigurations[0]!!.size)

        Assert.assertEquals(ConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is FrontDocumentPreviewFragment)

        Assert.assertEquals(ConfigurationSide.BACK,
            viewConfiguration.documentPreviewViewConfigurations[0]!![1].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![1].screen
                is BackDocumentPreviewFragment)

        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is DlFrontTransitionFragment)

        Assert.assertEquals(DocumentConfigurationSide.BACK,
            viewConfiguration.documentViewConfigurations[0]!![1].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![1].screen
                is DlBackTransitionFragment)
    }

    @Test
    fun getViewConfigurationBuilder_ReturnsViewForPhotoIdType() {
        val documentConfigurations = setUpDocumentConfiguration(PaycassoDocumentType.PhotoId)
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            transactionType = "transactionType",
            deviceId = "appDeviceId",
            appUserId = "appUserId",
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Only front on PhotoId type
        val expectedDocumentPreviewViewFrontConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewFrontConfiguration.previewSide = ConfigurationSide.FRONT
        expectedDocumentPreviewViewFrontConfiguration.screen = IdFrontTransitionFragment()

        val expectedDocumentPreviewViewBackConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewBackConfiguration.previewSide = ConfigurationSide.BACK
        expectedDocumentPreviewViewBackConfiguration.screen = IdBackTransitionFragment()

        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewFrontConfiguration,
            expectedDocumentPreviewViewBackConfiguration)

        val expectedDocumentFrontViewConfiguration = DocumentViewConfiguration()
        expectedDocumentFrontViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentFrontViewConfiguration.screen = IdFrontTransitionFragment()

        val expectedDocumentBackViewConfiguration = DocumentViewConfiguration()
        expectedDocumentBackViewConfiguration.documentSide = DocumentConfigurationSide.BACK
        expectedDocumentBackViewConfiguration.screen = IdBackTransitionFragment()

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentFrontViewConfiguration, expectedDocumentBackViewConfiguration)

        Assert.assertEquals(expectedDocumentPreviewConfigurationList.size,
            viewConfiguration.documentPreviewViewConfigurations[0]!!.size)
        Assert.assertEquals(expectedDocumentViewConfigurationList.size,
            viewConfiguration.documentViewConfigurations[0]!!.size)

        Assert.assertEquals(ConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is FrontDocumentPreviewFragment)

        Assert.assertEquals(ConfigurationSide.BACK,
            viewConfiguration.documentPreviewViewConfigurations[0]!![1].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![1].screen
                is BackDocumentPreviewFragment)

        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is IdFrontTransitionFragment)

        Assert.assertEquals(DocumentConfigurationSide.BACK,
            viewConfiguration.documentViewConfigurations[0]!![1].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![1].screen
                is IdBackTransitionFragment)
    }

    @Test
    fun getViewConfigurationBuilder_ReturnsViewForPassportDocumentType() {
        val documentConfigurations = setUpDocumentConfiguration(PaycassoDocumentType.Passport)
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            transactionType = "transactionType",
            deviceId = "appDeviceId",
            appUserId = "appUserId",
            transactionReference = "transactionReference"
        )
        val viewConfiguration = paycassoViewConfiguration
            .getViewConfigurationBuilder(documentConfigurations, externalReferences)

        // Setting up previewView and View configurations
        // Only front on Passport type
        val expectedDocumentPreviewViewConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewConfiguration.previewSide = ConfigurationSide.FRONT
        expectedDocumentPreviewViewConfiguration.screen = PassportTransitionFragment()

        val expectedDocumentViewConfiguration = DocumentViewConfiguration()
        expectedDocumentViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentViewConfiguration.screen = FrontDocumentPreviewFragment()


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
        Assert.assertEquals(ConfigurationSide.FRONT,
            viewConfiguration.documentPreviewViewConfigurations[0]!![0].previewSide)
        Assert.assertTrue(viewConfiguration.documentPreviewViewConfigurations[0]!![0].screen
                is FrontDocumentPreviewFragment)

        // Assert documentViewConfigurations fields
        Assert.assertEquals(DocumentConfigurationSide.FRONT,
            viewConfiguration.documentViewConfigurations[0]!![0].documentSide)
        Assert.assertTrue(viewConfiguration.documentViewConfigurations[0]!![0].screen
                is PassportTransitionFragment)

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
