package com.nhs.online.nhsonline.interfaces

import com.paycasso.sdk.api.flow.PaycassoFlowCallback
import com.paycasso.sdk.api.flow.model.AbstractPaycassoFlowRequest
import com.paycasso.sdk.api.flow.model.Credentials
import com.paycasso.sdk.api.flow.model.FlowConfiguration
import com.paycasso.sdk.api.flow.view.ViewConfiguration

interface IPaycassoFlow {
    fun start(
        credentials: Credentials,
        flowRequest: AbstractPaycassoFlowRequest,
        flowCallback: PaycassoFlowCallback,
        flowConfiguration: FlowConfiguration,
        viewConfiguration: ViewConfiguration
    )
}