package com.nhs.online.nhsonline.registration

import android.util.Log
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.data.*
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.paycasso.sdk.api.flow.enums.*
import com.paycasso.sdk.api.flow.model.BasePaycassoFlowRequest
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.model.FlowConfiguration
import com.paycasso.sdk.api.flow.model.SessionTokenCredentials
import com.paycasso.sdk.api.flow.view.ViewConfiguration
import com.paycasso.sdk.api.flow.view.configuration.*
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.EChipScreenType
import com.paycasso.sdk.api.flow.view.configuration.enums.PreviewConfigurationSide
import com.paycasso.view.*
import com.paycasso.view.nhs.*
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import paycasso.TransactionData
import java.util.HashMap

@RunWith(RobolectricTestRunner::class)
class PaycassoServiceTests {
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var paycassoService: PaycassoService
    private lateinit var flowMock: IPaycassoFlow
    private lateinit var flowCallbackMock: NhsAppPaycassoFlowCallback

    @Before
    fun setup(){
       appWebInterfaceMock = mock()
        flowMock = mock()
       paycassoService = PaycassoService(flowMock)
    }

    @Test
    fun start_startsThePaycassoFlow() {

        val success: (PaycassoCallbackResponse) -> Unit = mock()
        val failure: (PaycassoCallbackResponse) -> Unit = mock()
        val paycassoData = PaycassoData(
            credentials = PaycassoCredentials(
                hostUrl = "https://paycassoHostUrl.com",
                token = "fakeToken123"
            ),
            externalReferences = PaycassoExternalReferences(
                consumerReference = "consumerReference",
                transactionReference = "transactionReference",
                appUserId = "appUserId",
                deviceId = "deviceId",
                hasNfcJourney = false,
                transactionType = "DocuSure"
            ),
            transactionDetails = PaycassoTransactionDetails(
                documentType = PaycassoDocumentType.Passport
            )
        )

        flowCallbackMock = mock()

        val documentConfigurations = arrayListOf(
            DocumentConfiguration(
                PaycassoDocumentType.Passport.mrzLocation,
                PaycassoDocumentType.Passport.barcodeLocation,
                PaycassoDocumentType.Passport.faceLocation,
                PaycassoDocumentType.Passport.documentShape,
                PaycassoDocumentType.Passport.isBothSides,
                PaycassoDocumentType.Passport.eChipPresence,
                PaycassoDocumentType.Passport.docCheck ,
                PaycassoDocumentType.Passport.documentName

            )
        )
        val expectedFlowRequest = BasePaycassoFlowRequest()
        expectedFlowRequest.documentConfigurationList = documentConfigurations
        expectedFlowRequest.externalDeviceId = paycassoData.externalReferences.deviceId
        expectedFlowRequest.externalAppUserId = paycassoData.externalReferences.appUserId
        expectedFlowRequest.transactionType = TransactionType.DOCUSURE
        expectedFlowRequest.externalTransactionReference = paycassoData.externalReferences.transactionReference
        expectedFlowRequest.externalConsumerReference = paycassoData.externalReferences.consumerReference

        val expectedFlowConfiguration = FlowConfiguration()
        expectedFlowConfiguration.displayCancelButton = true
        expectedFlowConfiguration.isDisplayDocumentPreview = true

        val expectedCredentials = SessionTokenCredentials()
        expectedCredentials.hostUrl = "https://paycassoHostUrl.com"
        expectedCredentials.token = "fakeToken123"

        // Setting up views
        val expectedDocumentPreviewViewFrontConfiguration = DocumentPreviewViewConfiguration()
        expectedDocumentPreviewViewFrontConfiguration.previewSide = PreviewConfigurationSide.FRONT
        expectedDocumentPreviewViewFrontConfiguration.screen = NhsDocumentPreviewViewFragment()

        val expectedDocumentViewConfiguration = DocumentViewConfiguration()
        expectedDocumentViewConfiguration.documentSide = DocumentConfigurationSide.FRONT
        expectedDocumentViewConfiguration.screen = NhsPassportTransitionFragment()

        val expectedDocumentPreviewConfigurationList = arrayListOf(
            expectedDocumentPreviewViewFrontConfiguration
        )

        val expectedDocumentViewConfigurationList = arrayListOf(
            expectedDocumentViewConfiguration
        )

        val documentPreviewViewConfigurations =
            HashMap<Int, List<DocumentPreviewViewConfiguration>>()
        documentPreviewViewConfigurations[1] = expectedDocumentPreviewConfigurationList

        val documentViewConfigurations = HashMap<Int, List<DocumentViewConfiguration>>()
        documentViewConfigurations[1] = expectedDocumentViewConfigurationList

        val faceViewConfiguration = FaceViewConfiguration()
        faceViewConfiguration.screen = FaceTransitionFragment()

        val finishViewConfiguration = FinishViewConfiguration()
        finishViewConfiguration.screen = NhsFinishTransitionFragment()

        val eChipViewConfigurations = HashMap<EChipScreenType, EChipViewConfiguration>()
        val configuration1 = EChipViewConfiguration()
        configuration1.setScreenType(EChipScreenType.NO_NFC)
        configuration1.setScreen(NhsEChipNoNfcFragment())

        val configuration2 = EChipViewConfiguration()
        configuration2.setScreenType(EChipScreenType.ERROR)
        configuration2.setScreen(NhsEChipErrorFragment())

        val configuration4 = EChipViewConfiguration()
        configuration4.setScreenType(EChipScreenType.HINT)
        configuration4.setScreen(NhsEChipHintFragment())

        val configuration5 = EChipViewConfiguration()
        configuration5.setScreenType(EChipScreenType.PROCESSING)
        configuration5.setScreen(NhsEChipProcessingFragment())

        val configuration6 = EChipViewConfiguration()
        configuration6.setScreenType(EChipScreenType.MRZ_READING)
        configuration6.setScreen(NhsEChipMrzReadingFragment())

        eChipViewConfigurations[EChipScreenType.NO_NFC] = configuration1
        eChipViewConfigurations[EChipScreenType.ERROR] = configuration2
        eChipViewConfigurations[EChipScreenType.HINT] = configuration4
        eChipViewConfigurations[EChipScreenType.PROCESSING] = configuration5
        eChipViewConfigurations[EChipScreenType.MRZ_READING] = configuration6

        val expectedViewConfiguration = ViewConfiguration()
        expectedViewConfiguration.documentPreviewViewConfigurations = documentPreviewViewConfigurations
        expectedViewConfiguration.eChipViewConfigurations = eChipViewConfigurations
        expectedViewConfiguration.faceViewConfiguration = faceViewConfiguration
        expectedViewConfiguration.documentViewConfigurations = documentViewConfigurations
        expectedViewConfiguration.finishViewConfiguration = finishViewConfiguration


        paycassoService.start(paycassoData, success, failure)
        val credentialsArgumentCaptor = argumentCaptor<SessionTokenCredentials>()
        val flowRequestArgumentCaptor = argumentCaptor<BasePaycassoFlowRequest>()
        val viewConfigurationArgumentCaptor = argumentCaptor<ViewConfiguration>()
        val flowConfigurationArgumentCaptor = argumentCaptor<FlowConfiguration>()
        val callbackArgumentCaptor = argumentCaptor<NhsAppPaycassoFlowCallback>()

        verify(flowMock).start(
            credentialsArgumentCaptor.capture(),
            flowRequestArgumentCaptor.capture(),
            callbackArgumentCaptor.capture(),
            flowConfigurationArgumentCaptor.capture(),
            viewConfigurationArgumentCaptor.capture())

        Assert.assertEquals("https://paycassoHostUrl.com", credentialsArgumentCaptor.firstValue.hostUrl)
        Assert.assertEquals("fakeToken123", credentialsArgumentCaptor.firstValue.token)

        Assert.assertFalse(flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].isBothSides)
        Assert.assertFalse(flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].echipPresence)
        Assert.assertTrue(flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].docCheck)
        Assert.assertEquals(DocumentShape.PASSPORT,
            flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].documentShape)
        Assert.assertEquals(FaceLocation.FRONT,
            flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].faceLocation)
        Assert.assertEquals(MrzLocation.NO,
            flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].mrzLocation)
        Assert.assertEquals(BarcodeLocation.NO,
            flowRequestArgumentCaptor.firstValue.documentConfigurationList[0].barcodeLocation)

        Assert.assertTrue(flowConfigurationArgumentCaptor.firstValue.displayCancelButton)
        Assert.assertTrue(flowConfigurationArgumentCaptor.firstValue.displayDocumentPreview)

        Assert.assertTrue(viewConfigurationArgumentCaptor.firstValue.finishViewConfiguration.screen
                is NhsFinishTransitionFragment)
        Assert.assertTrue(viewConfigurationArgumentCaptor.firstValue.faceViewConfiguration.screen
                is FaceTransitionFragment)
        Assert.assertEquals(viewConfigurationArgumentCaptor.firstValue.
            documentViewConfigurations[0]!![0].documentSide, DocumentConfigurationSide.FRONT)
        Assert.assertTrue(viewConfigurationArgumentCaptor.firstValue.
            documentViewConfigurations[0]!![0].screen is NhsPassportTransitionFragment)
        Assert.assertEquals(viewConfigurationArgumentCaptor.firstValue.
            documentPreviewViewConfigurations[0]!![0].previewSide, PreviewConfigurationSide.FRONT)
        Assert.assertTrue(viewConfigurationArgumentCaptor.firstValue.
            documentPreviewViewConfigurations[0]!![0].screen is NhsDocumentPreviewViewFragment)
    }
    
    @Test
    fun getDocumentConfiguration_Returns_CorrectConfiguration() {
        val documentType = PaycassoDocumentType.Passport
        val documentToCheck = paycassoService.getDocumentConfiguration(documentType)[0]
        Assert.assertEquals(documentType.documentName, documentToCheck.documentName)
        Assert.assertEquals(documentType.barcodeLocation, documentToCheck.barcodeLocation)
        Assert.assertEquals(documentType.docCheck, documentToCheck.docCheck)
        Assert.assertEquals(documentType.eChipPresence, documentToCheck.echipPresence)
        Assert.assertEquals(documentType.faceLocation, documentToCheck.faceLocation)
        Assert.assertEquals(documentType.isBothSides, documentToCheck.isBothSides)

    }

    @Test
    fun buildFlowRequest_Returns_Correct_FlowRequest() {
        val documentConfiguration = DocumentConfiguration(
            MrzLocation.FRONT,
            BarcodeLocation.NO,
            FaceLocation.NO,
            DocumentShape.PASSPORT,
            false,
            true,
            true,
            "Test Document")
        val externalReferences = PaycassoExternalReferences(
            consumerReference = "consumerReference",
            transactionReference = "transactionReference",
            appUserId = "appUserId",
            deviceId = "deviceId",
            hasNfcJourney = false,
            transactionType = "DocuSure"
        )
        val documentConfigurations = arrayListOf(documentConfiguration)
        val flowRequest = paycassoService.buildFlowRequest(documentConfigurations, externalReferences).build()

        Assert.assertEquals("appUserId", flowRequest.externalAppUserId)
        Assert.assertEquals("consumerReference", flowRequest.externalConsumerReference)
        Assert.assertEquals("transactionReference", flowRequest.externalTransactionReference)
        Assert.assertEquals("deviceId", flowRequest.externalDeviceId)
    }
}
