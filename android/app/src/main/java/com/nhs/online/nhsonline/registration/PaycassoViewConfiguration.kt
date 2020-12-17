package com.nhs.online.nhsonline.registration

import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.paycasso.sdk.api.flow.builders.ViewConfigurationBuilder
import com.paycasso.sdk.api.flow.enums.TransactionType
import com.paycasso.sdk.api.flow.model.DocumentConfiguration
import com.paycasso.sdk.api.flow.view.configuration.*
import com.paycasso.sdk.api.flow.view.configuration.enums.DocumentConfigurationSide
import com.paycasso.sdk.api.flow.view.configuration.enums.ConfigurationSide
import com.paycasso.sdk.api.flow.view.screen.TransitionViewFragment
import com.paycasso.view.nhs.capturecontrol.BaseCaptureControlFragment
import com.paycasso.view.nhs.capturecontrol.DlCaptureControlFragment
import com.paycasso.view.nhs.capturecontrol.IdCaptureControlFragment
import com.paycasso.view.nhs.capturecontrol.PassportCaptureControlFragment
import com.paycasso.view.nhs.documentpreview.BackDocumentPreviewFragment
import com.paycasso.view.nhs.documentpreview.FrontDocumentPreviewFragment
import com.paycasso.view.nhs.finish.FinishTransitionFragment
import com.paycasso.view.nhs.transition.*
import kotlin.collections.ArrayList

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
                configurationList,
                externalReferences.transactionType)
            PaycassoDocumentType.PhotoId -> getViewConfigurationBuilderId(
                configurationList = configurationList,
                transactionType = externalReferences.transactionType
            )
        }

    private fun getViewConfigurationBuilderPassport(configurationList: ArrayList<DocumentConfiguration>,
                                                    transactionType: String): ViewConfigurationBuilder {

        // Information screen for Passport
        // In the passport journey we only scan the front of the passport
        val documentViewConfigurationList = arrayListOf(
            getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
                PassportTransitionFragment())
        )

        // 'Check the picture' screen
        val documentPreviewViewConfigurationList = arrayListOf(
            getDocumentPreviewConfiguration(ConfigurationSide.FRONT)
        )

        // Overlay on camera
        val frontDocumentSide = getCaptureControlConfiguration(ConfigurationSide.FRONT,
            PassportCaptureControlFragment())
        

        return setupConfiguration(configurationList,
            listOf(frontDocumentSide),
            documentViewConfigurationList,
            documentPreviewViewConfigurationList,
            transactionType)
    }

    private fun getViewConfigurationBuilderId(configurationList: ArrayList<DocumentConfiguration>,
                                              transactionType: String): ViewConfigurationBuilder {

        // Information screen for front of Id
        val firstFrontDocument = getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
            IdFrontTransitionFragment())

        // Information screen for back of Id
        val firstBackDocument = getDocumentViewConfiguration(DocumentConfigurationSide.BACK,
            IdBackTransitionFragment())

        // 'Check the picture' screen for front of Id
        val firstFrontDocumentPreview = getDocumentPreviewConfiguration(ConfigurationSide.FRONT)

        // 'Check the picture' for back of Id
        val firstBackDocumentPreview = getDocumentPreviewConfiguration(ConfigurationSide.BACK)

        // Overlay on camera for front
        val frontDocumentSide = getCaptureControlConfiguration(ConfigurationSide.FRONT,
            controlFragment = IdCaptureControlFragment())

        // Overlay on camera for back
        val backDocumentSide = getCaptureControlConfiguration(ConfigurationSide.BACK,
            IdCaptureControlFragment())

        return setupConfiguration(configurationList,
            arrayListOf(frontDocumentSide, backDocumentSide),
            arrayListOf(firstFrontDocument, firstBackDocument),
            arrayListOf(firstFrontDocumentPreview, firstBackDocumentPreview),
            transactionType)
    }

    private fun getViewConfigurationBuilderDriversLicence(configurationList: ArrayList<DocumentConfiguration>,
                                              transactionType: String): ViewConfigurationBuilder {

        // Information screen to scan front of DL
        val firstFrontDocument = getDocumentViewConfiguration(DocumentConfigurationSide.FRONT,
            DlFrontTransitionFragment())

        // Information screen to scan back of DL
        val firstBackDocument = getDocumentViewConfiguration(DocumentConfigurationSide.BACK,
            DlBackTransitionFragment())

        // 'Check the picture' screen for front
        val firstFrontDocumentPreview = getDocumentPreviewConfiguration(ConfigurationSide.FRONT)

        // 'Check the picture' screen for back
        val firstBackDocumentPreview = getDocumentPreviewConfiguration(ConfigurationSide.BACK)

        // Camera overlay screens for front and back
        val frontDocumentSide = getCaptureControlConfiguration(ConfigurationSide.FRONT,
            DlCaptureControlFragment())

        val backDocumentSide = getCaptureControlConfiguration(ConfigurationSide.BACK,
            DlCaptureControlFragment())

        return setupConfiguration(configurationList,
            arrayListOf(frontDocumentSide, backDocumentSide),
            arrayListOf(firstFrontDocument, firstBackDocument),
            arrayListOf(firstFrontDocumentPreview, firstBackDocumentPreview),
            transactionType)
    }

    private fun setupConfiguration(configurationList: ArrayList<DocumentConfiguration>,
                                   captureControlViewFragmentList: List<DocumentCaptureControlViewConfiguration>,
                                   documentViewConfigurationList: ArrayList<DocumentViewConfiguration>,
                                   documentPreviewViewConfigurationList: ArrayList<DocumentPreviewViewConfiguration>,
                                   transactionType: String = "")
            : ViewConfigurationBuilder {
        val documentViewConfigurations = hashMapOf<Int, List<DocumentViewConfiguration>>()
        val captureControlViewConfigurations = hashMapOf<Int, List<DocumentCaptureControlViewConfiguration>>()
        val documentPreviewViewConfigurations =
            hashMapOf<Int, List<DocumentPreviewViewConfiguration>>()

        captureControlViewConfigurations[captureControlViewConfigurations.size] =
            captureControlViewFragmentList

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
            .documentViewConfigurations(documentViewConfigurations)
            .finishViewConfiguration(finishViewConfiguration)
            .documentPreviewViewConfigurations(documentPreviewViewConfigurations)
            .documentCaptureControlViewConfigurations(captureControlViewConfigurations)
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

    private fun getDocumentPreviewConfiguration(side: ConfigurationSide)
            : DocumentPreviewViewConfiguration {
        val document = DocumentPreviewViewConfiguration()
        document.previewSide = side

        document.screen =
            if (side == ConfigurationSide.FRONT) FrontDocumentPreviewFragment()
            else BackDocumentPreviewFragment()

        return document
    }

    private fun getCaptureControlConfiguration(side: ConfigurationSide,
                                               controlFragment: BaseCaptureControlFragment):
            DocumentCaptureControlViewConfiguration {

        val documentCaptureControlViewConfiguration = DocumentCaptureControlViewConfiguration()
        documentCaptureControlViewConfiguration.documentSide = side
        documentCaptureControlViewConfiguration.screen = controlFragment
        return documentCaptureControlViewConfiguration

    }


    private val finishViewConfiguration: FinishViewConfiguration
        get() {
            val finishViewConfiguration = FinishViewConfiguration()
            finishViewConfiguration.setScreen(FinishTransitionFragment())
            return finishViewConfiguration
        }
}
