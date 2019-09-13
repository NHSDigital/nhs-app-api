package com.nhs.online.nhsonline.clients

import com.google.firebase.iid.FirebaseInstanceId

class FirebaseClient {

    val instanceId get() = FirebaseInstanceId.getInstance().instanceId
}
