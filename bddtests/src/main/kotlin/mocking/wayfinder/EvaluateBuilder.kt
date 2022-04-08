package mocking.wayfinder

import mocking.models.Mapping
import org.apache.http.HttpStatus
import utils.SerenityHelpers

private const val TIMEOUT_DELAY_IN_MILLISECONDS = 30000;

class EvaluateBuilder
    : WayfinderMappingBuilder("GET","/fhir/secondary-care/summary/\$evaluate") {

    init {
        requestBuilder.andHeader("X-NHS-Number", SerenityHelpers.getPatient().nhsNumbers[0])
    }

    fun returnAfterThirtySecondsForTimeout(): Mapping {
        return respondWith(HttpStatus.SC_OK){
            andDelay(TIMEOUT_DELAY_IN_MILLISECONDS)
        }
    }

    fun returnInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR){}
    }

    fun returnNoReferralsOrAppointments(): Mapping {
        val response = """
        {
            "referrals": [],
            "upcomingAppointments": []
        } 
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                .build()
        }
    }

    fun returnReferralsAndNoAppointments(): Mapping {
        val response = """
            {
            "referrals": [
            {
                "referralId": "1500 5957 5801",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "InReview",
                "reviewDueDate": "2125-04-06T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5802",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "BookableWasCancelled",
                "deepLinkUrl": "www.google.com",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5803",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "Bookable",
                "reviewDueDate": "2022-04-06T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5804",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "InReview",
                "reviewDueDate": "2022-04-01T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            ],
            "upcomingAppointments": []
        } 
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }

    fun returnReferralsAndAppointments(): Mapping {
        val response = """
        {
            "referrals": [
            {
                "referralId": "1500 5957 5801",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "InReview",
                "reviewDueDate": "2125-04-06T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5802",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "BookableWasCancelled",
                "deepLinkUrl": "www.google.com",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5803",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "Bookable",
                "reviewDueDate": "2022-04-06T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            {
                "referralId": "1500 5957 5804",
                "referredDateTime": "2022-03-20T12:18:10.0150205+00:00",
                "serviceSpeciality": "Cardiology",
                "referrerOrganisation": "Mahogany GP Surgery",
                "status": "InReview",
                "reviewDueDate": "2022-04-01T12:18:10.0151477+00:00",
                "provider": "Ers"
            },
            ],
            "upcomingAppointments": [
            {
                "appointmentId": "8219 2357 5528",
                "appointmentDateTime": null,
                "locationDescription": "General, The Willows, Croydon University Hospital, RJ6 5EU",
                "serviceSpeciality": null,
                "deepLinkUrl": "http://stubs.local.bitraft.io:8080/upcoming-appointments/pkb?reference=8219 2357 5528",
                "provider": "Pkb"
            },
            {
                "appointmentId": "9320 3468 6639",
                "appointmentDateTime": "2022-04-20T12:18:10.0150205+00:00",
                "locationDescription": "Neurology, The Willows, Croydon University Hospital, RJ6 5EU",
                "serviceSpeciality": null,
                "deepLinkUrl": "http://stubs.local.bitraft.io:8080/upcoming-appointments/pkb?reference=9320 3468 6639",
                "provider": "Pkb"
            },
            {
                "appointmentId": "1001 3003 5005",
                "appointmentDateTime": null,
                "locationDescription": "General, The Oaks, Wembley University Hospital, WJ7 6EU",
                "serviceSpeciality": null,
                "deepLinkUrl": "http://stubs.local.bitraft.io:8080/upcoming-appointments/pkb?reference=1001 3003 5005",
                "provider": "Ers"
            },
            {
                "appointmentId": "2002 4004 6006",
                "appointmentDateTime": "2022-04-20T12:18:10.0150205+00:00",
                "locationDescription": "Cardiology, The Oaks, Wembley University Hospital, WJ7 6EU",
                "serviceSpeciality": null,
                "deepLinkUrl": "http://stubs.local.bitraft.io:8080/upcoming-appointments/pkb?reference=2002 4004 6006",
                "provider": "Ers"
            }
            ]
        } 
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }
}
