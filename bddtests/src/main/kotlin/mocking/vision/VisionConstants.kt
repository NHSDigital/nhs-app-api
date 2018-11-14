package mocking.vision

import mocking.vision.helpers.VisionConstantsHelper.Companion.getBaseVisionResponse
import mocking.vision.helpers.VisionConstantsHelper.Companion.setContextOnServiceContent
import mocking.vision.models.appointments.AvailableAppointmentsResponse
import mocking.vision.models.appointments.BookedAppointmentsResponse

object VisionConstants {

    // Service names and versions
    const val configurationName: String = "VOS.GetConfiguration"
    const val configurationVersion: String = "2.3.0"

    const val prescriptionHistory: String = "VONREP.GetHistory"
    const val prescriptionHistoryVersion: String = "2.0.0"

    const val eligibleRepeats: String = "VONREP.GetEligibleRepeats"
    const val eligibleRepeatsVersion: String = "2.0.0"

    const val newPrescription: String = "VONREP.NewPrescription"
    const val newPrescriptionVersion: String = "2.0.0"

    const val demographicsName: String = "VODEM.GetDemographics"
    const val demographicsVersion: String = "2.0.0"

    const val patientDataName: String = "VOS.GetPatientData"
    const val patientDataVersion: String = "2.1.0"

    const val existingAppointmentsName: String = "VOAPP.GetExistingAppointments"
    const val existingAppointmentsVersion: String = "2.0.0"

    const val availableAppointmentsName: String = "VOAPP.GetAvailableAppointment"
    const val availableAppointmentsVersion: String = "2.0.0"

    const val cancelAppointmentsName: String = "VOAPP.CancelAppointment"
    const val cancelAppointmentsVersion: String = "2.0.0"

    const val allergiesView: String  = "VPS_ALLERGIES"
    const val medicationsView: String = "VPS_MEDICATIONS"
    const val immunisationsView: String = "PROCEDURES"
    const val problemsView: String = "PROBLEMS"

    const val bookAppointmentName: String = "VOAPP.BookAppointment"
    const val bookAppointmentVersion: String = "2.0.0"

    const val htmlResponseFormat: String = "HTML"
    const val xmlResponseFormat: String = "XML"

    const val defaultOwnerId: String = "ALL"

    var gpAppointmentsDisabled: String = "gpAppointmentsDisabled"

    fun getVisionExistingAppointmentsResponse(serviceContent: String,
                                              serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        val response = setContextOnServiceContent(serviceContent, BookedAppointmentsResponse.name)

        return getBaseVisionResponse(response, serviceDefinition)
    }

    fun getVisionAvailableAppointmentsResponse(serviceContent: String,
                                               serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        val response = setContextOnServiceContent(serviceContent, AvailableAppointmentsResponse.name)

        return getBaseVisionResponse(response, serviceDefinition)
    }

    // Vision Get Patient Data
    fun getClinicalDataResponse(serviceContent: String,
                                serviceDefinition: mocking.vision.models.ServiceDefinition): String {
        return getBaseVisionResponse("<vision:record>$serviceContent</vision:record>", serviceDefinition)
    }

    // Vision Demographics
    fun getVisionDemographicsResponse(serviceContent: String,
                                      serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        val response = setContextOnServiceContent(serviceContent, "demographics")

        return getBaseVisionResponse(response, serviceDefinition)
    }

    var linkAccount: String = "VOS.LinkAccount"
    var linkAccountVersion: String = "1.0.0"

    // Vision Response
    fun getVisionResponse(serviceContent: String, serviceDefinition: mocking.vision.models.ServiceDefinition):
            String {

        val response = serviceContent
                .replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>", "")
                .replace("</", "</vision:")
                .replace("<", "<vision:")
                .replace("vision:/", "/")

        return getBaseVisionResponse(response, serviceDefinition)
    }
}
