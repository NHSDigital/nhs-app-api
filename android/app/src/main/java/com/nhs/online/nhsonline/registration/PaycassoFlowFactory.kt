package com.nhs.online.nhsonline.registration

import android.content.Context
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow
import java.util.logging.Logger

class PaycassoFlowFactory(private val logger: Logger) {
    fun getFlow(context: Context): IPaycassoFlow {
        return PaycassoFlowProxy(context, logger)
    }
}
