package com.nhs.online.nhsonline.support.uiinteraction

import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel

interface IHeaderStrategy {
    fun apply(integrationLevel: IntegrationLevel)
}
