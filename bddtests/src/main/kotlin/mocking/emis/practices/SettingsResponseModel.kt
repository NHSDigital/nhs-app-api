package mocking.emis.practices

import mocking.emis.models.InputRequirements
import mocking.emis.models.Messages
import mocking.emis.models.Services

data class SettingsResponseModel (
        var services: Services = Services(),
        var messages: Messages = Messages(),
        var inputRequirements: InputRequirements = InputRequirements()
)