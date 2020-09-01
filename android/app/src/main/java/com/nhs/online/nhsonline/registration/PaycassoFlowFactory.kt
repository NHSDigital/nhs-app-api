package com.nhs.online.nhsonline.registration

import android.content.Context
import com.nhs.online.nhsonline.interfaces.IPaycassoFlow

class PaycassoFlowFactory {

    fun getFlow(context: Context): IPaycassoFlow {
        return PaycassoFlowProxy(context)
    }
}