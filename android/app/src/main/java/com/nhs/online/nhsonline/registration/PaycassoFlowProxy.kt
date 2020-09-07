package com.nhs.online.nhsonline.registration

import android.content.Context
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow
import com.paycasso.sdk.api.flow.PaycassoFlow
import com.paycasso.sdk.api.flow.PaycassoFlowCallback
import com.paycasso.sdk.api.flow.model.AbstractPaycassoFlowRequest
import com.paycasso.sdk.api.flow.model.Credentials
import com.paycasso.sdk.api.flow.model.FlowConfiguration
import com.paycasso.sdk.api.flow.model.FlowFailureResponse
import com.paycasso.sdk.api.flow.view.ViewConfiguration
import java.util.logging.Level
import java.util.logging.Logger

class PaycassoFlowProxy(private val context: Context, private val logger: Logger): IPaycassoFlow {
    override fun start(
        credentials: Credentials,
        flowRequest: AbstractPaycassoFlowRequest,
        flowCallback: PaycassoFlowCallback,
        flowConfiguration: FlowConfiguration,
        viewConfiguration: ViewConfiguration
    ) {
        try {
            PaycassoFlow.getInstance(context)
                .start(credentials, flowRequest, flowCallback, flowConfiguration, viewConfiguration)
        } catch (ex: UnsatisfiedLinkError) {
            logger.severe(
          "Failed to load native Paycasso SDK libraries - Current device will be " +
                "marked as not supported (Note: only ARM platforms are supported)")
            logger.log(Level.SEVERE, ex.message, ex)

            flowCallback.onFailure(
                FlowFailureResponse(
                    -10000,
                    "Paycasso SDK is not supported on this device (see device logs for more info)"))
        }
    }
}
