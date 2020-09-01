package com.nhs.online.nhsonline.registration

import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.paycasso.sdk.api.flow.builders.ViewConfigurationBuilder
import com.paycasso.sdk.api.flow.enums.MrzLocation
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.view.configuration.*
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.EChipScreenType
import com.paycasso.sdk.api.flow.view.configuration.enums.PreviewConfigurationSide
import com.paycasso.sdk.api.flow.view.screen.TransitionViewFragment
import com.paycasso.view.*
import paycasso.TransactionData
import java.util.ArrayList
import java.util.HashMap

class PaycassoViewConfiguration(
    private val documentType: PaycassoDocumentType
) {
    fun getViewConfigurationBuilder(
        configurationList: ArrayList<DocumentConfiguration>,
        externalReferences: PaycassoExternalReferences
    ) =
        
        when (documentType) {
            PaycassoDocumentType.DriversLicence -> getViewConfigurationBuilderId(
                true,
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
            getDocumentViewConfiguration(DocumentConfigurationSide.FRONT, FirstFrontTransitionFragment())
        )
        val documentPreviewViewConfigurationList = arrayListOf(
            getDocumentPreviewConfiguration(PreviewConfigurationSide.FRONT)
        )

        if(hasNfcJourney) {
            configurationList[0].echipPresence = true
            configurationList[0].mrzLocation = MrzLocation.FRONT
        }
        

        return setupConfiguration(configurationList,
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun getViewConfigurationBuilderId(hasBothSides: Boolean = false,
                                              configurationList: ArrayList<DocumentConfiguration>,
                                              transactionType: String): ViewConfigurationBuilder {
        val firstFrontDocument = getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
            FirstFrontTransitionFragment())
        val firstFrontDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.FRONT)
        val documentViewConfigurationList = arrayListOf(firstFrontDocument)
        val documentPreviewViewConfigurationList = arrayListOf(firstFrontDocumentPreview)

        if(hasBothSides) {
            configurationList[0].isBothSides = true
            val firstBackDocumentPreview = getDocumentPreviewConfiguration(PreviewConfigurationSide.BACK)
            val firstBackDocument = getDocumentViewConfiguration(DocumentConfigurationSide.BACK,
                FirstBackTransitionFragment())
            documentViewConfigurationList.add(firstBackDocument)
            documentPreviewViewConfigurationList.add(firstBackDocumentPreview)
         }

        return setupConfiguration(configurationList,
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun setupConfiguration(configurationList: ArrayList<DocumentConfiguration>,
                                   documentViewConfigurationList: ArrayList<DocumentViewConfiguration>,
                                   documentPreviewViewConfigurationList: ArrayList<DocumentPreviewViewConfiguration>,
                                   transactionType: String = "")
            : ViewConfigurationBuilder{
        val documentViewConfigurations = hashMapOf<Int, List<DocumentViewConfiguration>>()
        val documentPreviewViewConfigurations =
            hashMapOf<Int, List<DocumentPreviewViewConfiguration>>()

        documentViewConfigurations[documentViewConfigurations.size] =
            documentViewConfigurationList

        documentPreviewViewConfigurations[documentPreviewViewConfigurations.size] =
            documentPreviewViewConfigurationList

        val transactionDocumentType = when(transactionType){
            "InstaSure" -> TransactionData.TransactionRequest.TransactionType.InstaSure
            "VeriSure" -> TransactionData.TransactionRequest.TransactionType.VeriSure
            else -> TransactionData.TransactionRequest.TransactionType.DocuSure
        }

        return ViewConfigurationBuilder()
            .eChipViewConfigurations(eChipViewConfigurationMap)
            .documentViewConfigurations(documentViewConfigurations)
            .faceViewConfiguration(faceViewConfiguration)
            .finishViewConfiguration(finishViewConfiguration)
            .documentPreviewViewConfigurations(documentPreviewViewConfigurations)
            .transactionType(transactionDocumentType)
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
        document.screen = DocumentPreviewViewFragment()
        return document
    }


    private val finishViewConfiguration: FinishViewConfiguration
        get() {
            val finishViewConfiguration = FinishViewConfiguration()
            finishViewConfiguration.setScreen(FinishTransitionFragment())
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
            configuration1.setScreen(EChipNoNfcFragment())

            val configuration2 = EChipViewConfiguration()
            configuration2.setScreenType(EChipScreenType.ERROR)
            configuration2.setScreen(EChipErrorFragment())

            val configuration4 = EChipViewConfiguration()
            configuration4.setScreenType(EChipScreenType.HINT)
            configuration4.setScreen(EChipHintFragment())

            val configuration5 = EChipViewConfiguration()
            configuration5.setScreenType(EChipScreenType.PROCESSING)
            configuration5.setScreen(EChipProcessingFragment())

            val configuration6 = EChipViewConfiguration()
            configuration6.setScreenType(EChipScreenType.MRZ_READING)
            configuration6.setScreen(EChipMrzReadingFragment())

            eChipViewConfigurations[EChipScreenType.NO_NFC] = configuration1
            eChipViewConfigurations[EChipScreenType.ERROR] = configuration2
            eChipViewConfigurations[EChipScreenType.HINT] = configuration4
            eChipViewConfigurations[EChipScreenType.PROCESSING] = configuration5
            eChipViewConfigurations[EChipScreenType.MRZ_READING] = configuration6
            return eChipViewConfigurations
        }
}
