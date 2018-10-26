package mocking.vision

import mocking.vision.helpers.VisionConstantsHelper.Companion.setContextOnServiceContent
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

    const val allergiesView: String  = "VPS_ALLERGIES"
    const val medicationsView: String = "VPS_MEDICATIONS"
    const val immunisationsView: String = "PROCEDURES"
    const val problemsView: String = "PROBLEMS"

    const val htmlResponseFormat: String = "HTML"
    const val xmlResponseFormat: String = "XML"

    var gpAppointmentsDisabled: String = "gpAppointmentsDisabled"

    fun getVisionExistingAppointmentsResponse(serviceContent: String,
                                              serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        val response = setContextOnServiceContent(serviceContent, BookedAppointmentsResponse.name)

        return getBaseVisionResponse(response, serviceDefinition)
   }

    // Vision Get Patient Data
    fun getClinicalDataResponse(serviceContent: String,
                                serviceDefinition: mocking.vision.models.ServiceDefinition) : String {
        return getBaseVisionResponse("<vision:record>${serviceContent}</vision:record>", serviceDefinition)
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

    fun getBaseVisionResponse(response: String, serviceDefinition: mocking.vision.models.ServiceDefinition) : String {

        return "<soap:Envelope xmlns:urn=\"urn:vision\" " +
                "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "    <soap:Body>\n" +
                "        <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "            <vision:serviceDefinition>\n" +
                "                <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "                <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "            </vision:serviceDefinition>\n" +
                "            <vision:serviceHeader>\n" +
                "                <vision:outcome>\n" +
                "                    <vision:successful>true</vision:successful>\n" +
                "                </vision:outcome>\n" +
                "            </vision:serviceHeader>\n" +
                //           putting service content on one line as response can be raw text (avoiding new lines)
                "            <vision:serviceContent>" + response + "</vision:serviceContent>" +
                "        </vision:visionResponse>\n" +
                "    </soap:Body>\n" +
                "</soap:Envelope>"
    }
}
