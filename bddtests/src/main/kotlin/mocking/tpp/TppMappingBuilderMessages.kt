package mocking.tpp

import mocking.tpp.messages.MessagesBuilderTpp
import models.Patient

class TppMappingBuilderMessages {
     fun appointmentMessageRequest(patient: Patient) = MessagesBuilderTpp(patient)
}
