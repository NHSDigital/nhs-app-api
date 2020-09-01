package com.nhs.online.nhsonline.registration

import android.util.Log
import com.nhs.online.nhsonline.data.PaycassoCredentials
import com.nhs.online.nhsonline.data.PaycassoData
import com.nhs.online.nhsonline.data.PaycassoDocumentType
import com.nhs.online.nhsonline.data.PaycassoExternalReferences
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow
import com.paycasso.sdk.api.flow.builders.BasePaycassoFlowRequestBuilder
import com.paycasso.sdk.api.flow.builders.CredentialsBuilder
import com.paycasso.sdk.api.flow.model.*
import com.paycasso.sdk.api.flow.view.ViewConfiguration
import com.paycasso.sdk.exceptions.ConfigurationViewBuilderException
import com.paycasso.sdk.exceptions.RequestBuilderException
import paycasso.TransactionData.TransactionRequest.TransactionType.DocuSure
import java.lang.Exception
import java.util.*

private val TAG = PaycassoService::class.java.simpleName

class PaycassoService(
    private val paycassoFlow: IPaycassoFlow
) {
    fun start(paycassoData: PaycassoData, onSuccess: (PaycassoCallbackResponse) -> Unit,
              onFailure: (PaycassoCallbackResponse) -> Unit) {
        val documentTypes = paycassoData.transactionDetails.documentType

        val listDocumentConfigurations = getDocumentConfiguration(documentTypes)

        val flowBuilder = buildFlowRequest(
            listDocumentConfigurations,
            paycassoData.externalReferences)

        val credentialsBuilder = buildCredentials(paycassoData.credentials)

        val credentialsRequest: SessionTokenCredentials
        val flowRequest: BasePaycassoFlowRequest

        val viewConfiguration: ViewConfiguration
        try {
            credentialsRequest = credentialsBuilder.build()
            flowRequest = flowBuilder.build()
            viewConfiguration = PaycassoViewConfiguration(documentTypes)
                .getViewConfigurationBuilder(listDocumentConfigurations,
                    paycassoData.externalReferences)
                .build()

            paycassoFlow.start(credentialsRequest, flowRequest,
                NhsAppPaycassoFlowCallback(onSuccess, onFailure),
                flowConfiguration, viewConfiguration)
        } catch (e: ConfigurationViewBuilderException) {
            val paycassoError = PaycassoError(errorMessage = "ConfigurationViewBuilderException occurred: ${e.message}")
            onFailure(PaycassoCallbackResponse(paycassoError = paycassoError))
            Log.e(TAG, "ConfigurationViewBuilderException occurred:${e.message}")
            return
        } catch (e: RequestBuilderException) {
            val paycassoError = PaycassoError(errorMessage = "RequestBuilderException occurred: ${e.message}")
            onFailure(PaycassoCallbackResponse(paycassoError = paycassoError))
            Log.e(TAG, "RequestBuilderException occurred: ${e.message}")
            return
        } catch (e: Exception) {
            val paycassoError = PaycassoError(errorMessage = "Exception occurred: ${e.message}")
            onFailure(PaycassoCallbackResponse(paycassoError = paycassoError))
            Log.e(TAG, "Exception occurred: ${e.message}")
            return
        }
    }

    fun getDocumentConfiguration(documentTypes: PaycassoDocumentType): ArrayList<DocumentConfiguration>{
        val listDocumentConfigurations = ArrayList<DocumentConfiguration>()

        val documentConfiguration = DocumentConfiguration(
            documentTypes.mrzLocation,
            documentTypes.barcodeLocation,
            documentTypes.faceLocation,
            documentTypes.documentShape,
            documentTypes.isBothSides,
            documentTypes.eChipPresence,
            documentTypes.docCheck,
            documentTypes.documentName
        )

        listDocumentConfigurations.add(documentConfiguration)
        return listDocumentConfigurations
    }

    fun buildFlowRequest(listDocumentConfigurations: ArrayList<DocumentConfiguration>,
                                 externalReferences: PaycassoExternalReferences): BasePaycassoFlowRequestBuilder {
        return BasePaycassoFlowRequestBuilder()
            .externalConsumerReference(externalReferences.consumerReference)
            .externalTransactionReference(externalReferences.transactionReference)
            .externalAppUserId(externalReferences.appUserId)
            .externalDeviceId(externalReferences.deviceId)
            .transactionType(DocuSure)
            .documentConfigurationList(listDocumentConfigurations)
    }

     private fun buildCredentials(credentials: PaycassoCredentials): CredentialsBuilder{
        return CredentialsBuilder()
            .token(credentials.token)
            .hostUrl(credentials.hostUrl)
    }


    private val flowConfiguration: FlowConfiguration
        get() {
            val flowConfiguration = FlowConfiguration()

            flowConfiguration.isDisplayDocumentPreview = true
            flowConfiguration.isDisplayCancelButton = true
            flowConfiguration.receiveMrzData = true

            return flowConfiguration
        }
}

