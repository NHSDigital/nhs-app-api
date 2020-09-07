package com.nhs.online.nhsonline.registration

import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.paycasso.sdk.api.flow.builders.ViewConfigurationBuilder
import com.paycasso.sdk.api.flow.enums.MrzLocation
import com.paycasso.sdk.api.flow.enums.TransactionType
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.view.configuration.*
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.EChipScreenType
import com.paycasso.sdk.api.flow.view.configuration.enums.PreviewConfigurationSide
import com.paycasso.sdk.api.flow.view.screen.TransitionViewFragment
import com.paycasso.sdk.api.flow.view.screen.base.ViewFragment
import com.paycasso.view.FaceTransitionFragment
import com.paycasso.view.nhs.*
import java.util.*

class PaycassoViewConfiguration(
    private val documentType: PaycassoDocumentType
) {
    fun getViewConfigurationBuilder(
        configurationList: ArrayList<DocumentConfiguration>,
        externalReferences: PaycassoExternalReferences
    ) =
        
        when (documentType) {
            PaycassoDocumentType.DriversLicence -> getViewConfigurationBuilderDriversLicence(
                configurationList,
                externalReferences.transactionType)
            PaycassoDocumentType.Passport -> getViewConfigurationBuilderPassport(
                externalReferences.hasNfcJourney,
                configurationList,
                externalReferences.transactionType)
            PaycassoDocumentType.PhotoId -> getViewConfigurationBuilderId(
                configurationList = configurationList,
                transactionType = externalReferences.transactionType
            )
        }

    private fun getViewConfigurationBuilderPassport(hasNfcJourney: Boolean = false,
                                                    configurationList: ArrayList<DocumentConfiguration>,
                                                    transactionType: String): ViewConfigurationBuilder {
        val documentViewConfigurationList = arrayListOf(
            getDocumentViewConfiguration(DocumentConfigurationSide.FRONT, NhsPassportTransitionFragment())
        )
        val documentPreviewViewConfigurationList = arrayListOf(
            getDocumentPreviewConfiguration(PreviewConfigurationSide.FRONT)
        )

        if(hasNfcJourney) {
            configurationList[0].echipPresence = true
            configurationList[0].mrzLocation = MrzLocation.FRONT
        }
        

        return setupConfiguration(configurationList,
            NhsDefaultCaptureControlViewFragment(),
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun getViewConfigurationBuilderId(configurationList: ArrayList<DocumentConfiguration>,
                                              transactionType: String): ViewConfigurationBuilder {
        val firstFrontDocument = getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
            NhsIdFirstTransitionFragment())
        val firstFrontDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.FRONT)
        val documentViewConfigurationList = arrayListOf(firstFrontDocument)
        val documentPreviewViewConfigurationList = arrayListOf(firstFrontDocumentPreview)

        val firstBackDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.BACK)
        val firstBackDocument = getDocumentViewConfiguration(DocumentConfigurationSide.BACK,
            NhsIdSecondTransitionFragment())
        documentViewConfigurationList.add(firstBackDocument)
        documentPreviewViewConfigurationList.add(firstBackDocumentPreview)

        return setupConfiguration(configurationList,
            NhsDefaultCaptureControlViewFragment(),
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun getViewConfigurationBuilderDriversLicence(configurationList: ArrayList<DocumentConfiguration>,
                                              transactionType: String): ViewConfigurationBuilder {
        val firstFrontDocument = getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
            NhsDriverLicenceFirstTransitionFragment())
        val firstFrontDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.FRONT)
        val documentViewConfigurationList = arrayListOf(firstFrontDocument)
        val documentPreviewViewConfigurationList = arrayListOf(firstFrontDocumentPreview)

        val firstBackDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.BACK)
        val firstBackDocument = getDocumentViewConfiguration(DocumentConfigurationSide.BACK,
            NhsDriverLicenceSecondTransitionFragment())
        documentViewConfigurationList.add(firstBackDocument)
        documentPreviewViewConfigurationList.add(firstBackDocumentPreview)

        return setupConfiguration(configurationList,
            NhsDriverLicenceCaptureControlViewFragment(),
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun setupConfiguration(configurationList: ArrayList<DocumentConfiguration>,
                                   captureControlViewFragment: ViewFragment,
                                   documentViewConfigurationList: ArrayList<DocumentViewConfiguration>,
                                   documentPreviewViewConfigurationList: ArrayList<DocumentPreviewViewConfiguration>,
                                   transactionType: String = "")
            : ViewConfigurationBuilder{
        val documentViewConfigurations = hashMapOf<Int, List<DocumentViewConfiguration>>()
        val documentPreviewViewConfigurations =
            hashMapOf<Int, List<DocumentPreviewViewConfiguration>>()

        val captureControlViewConfiguration =
            CaptureControlViewConfiguration()
        captureControlViewConfiguration.setScreen(captureControlViewFragment)

        documentViewConfigurations[documentViewConfigurations.size] =
            documentViewConfigurationList

        documentPreviewViewConfigurations[documentPreviewViewConfigurations.size] =
            documentPreviewViewConfigurationList

        val transactionDocumentType = when(transactionType){
            "InstaSure" -> TransactionType.INSTASURE
            "VeriSure" -> TransactionType.VERISURE
            else -> TransactionType.DOCUSURE
        }

        return ViewConfigurationBuilder()
            .eChipViewConfigurations(eChipViewConfigurationMap)
            .documentViewConfigurations(documentViewConfigurations)
            .faceViewConfiguration(faceViewConfiguration)
            .finishViewConfiguration(finishViewConfiguration)
            .documentPreviewViewConfigurations(documentPreviewViewConfigurations)
            .transactionType(transactionDocumentType)
            .captureViewConfiguration(captureControlViewConfiguration)
            .documentConfigurationList(configurationList)
    }

    private fun getDocumentViewConfiguration(side: DocumentConfigurationSide,
                                         screen: TransitionViewFragment)
            : DocumentViewConfiguration {
        val document = DocumentViewConfiguration()
        document.documentSide = side
        document.screen = screen
        return document
    }

    private fun getDocumentPreviewConfiguration(side: PreviewConfigurationSide)
            : DocumentPreviewViewConfiguration {
        val document = DocumentPreviewViewConfiguration()
        document.previewSide = side
        document.screen = NhsDocumentPreviewViewFragment()
        return document
    }


    private val finishViewConfiguration: FinishViewConfiguration
        get() {
            val finishViewConfiguration = FinishViewConfiguration()
            finishViewConfiguration.setScreen(NhsFinishTransitionFragment())
            return finishViewConfiguration
        }

    private val faceViewConfiguration: FaceViewConfiguration
        get() {
            val faceViewConfiguration = FaceViewConfiguration()
            faceViewConfiguration.setScreen(FaceTransitionFragment())
            return faceViewConfiguration
        }

    private val eChipViewConfigurationMap: Map<EChipScreenType, EChipViewConfiguration>
        get() {
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
            return eChipViewConfigurations
        }
}
