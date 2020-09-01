package com.nhs.online.nhsonline.registration

import android.content.Context
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow
import com.paycasso.sdk.api.flow.PaycassoFlow
import com.paycasso.sdk.api.flow.PaycassoFlowCallback
import com.paycasso.sdk.api.flow.model.AbstractPaycassoFlowRequest
import com.paycasso.sdk.api.flow.model.Credentials
import com.paycasso.sdk.api.flow.model.FlowConfiguration
import com.paycasso.sdk.api.flow.view.ViewConfiguration

class PaycassoFlowProxy(context: Context): IPaycassoFlow {

    private val paycassoFlow = PaycassoFlow.getInstance(context)

    override fun start(
        credentials: Credentials,
        flowRequest: AbstractPaycassoFlowRequest,
        flowCallback: PaycassoFlowCallback,
        flowConfiguration: FlowConfiguration,
        viewConfiguration: ViewConfiguration
    ) {
        paycassoFlow.start(credentials, flowRequest, flowCallback, flowConfiguration, viewConfiguration)
    }
}