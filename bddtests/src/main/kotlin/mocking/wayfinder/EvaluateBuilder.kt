@file:Suppress("MaxLineLength")
package mocking.wayfinder

import mocking.models.Mapping
import org.apache.http.HttpStatus
import utils.SerenityHelpers
import java.net.URLEncoder

private const val TIMEOUT_DELAY_IN_MILLISECONDS = 30000;

/*
 * Base 64 encoding of the Aggregators Target Identifier within the scope of BaRS (Booking and Referrals Standard)
 *
 * {
 *     "system": "urn:ietf:rfc:3986",
 *     "value": "db71698b-cd7c-4dd5-95c4-0aa9776595f5"
 * }
 *
 */
private const val NHSD_TARGET_IDENTIFIER =
    "ewrCoCDCoCAic3lzdGVtIjogInVybjppZXRmOnJmYzozOTg2IiwKwqAgwqAgInZh" +
    "bHVlIjogImRiNzE2OThiLWNkN2MtNGRkNS05NWM0LTBhYTk3NzY1OTVmNSIKfQ=="

private fun getEncodedPatientIdQueryName() = URLEncoder.encode("patient:identifier", "UTF-8")
private fun getPatientIdQueryValue() = "https://fhir.nhs.uk/Id/nhs-number|${SerenityHelpers.getPatient().nhsNumbers[0]}"
private fun getPath(): String {
    val path = "/patient-care-aggregator-api/aggregator/events"
    val queryName = getEncodedPatientIdQueryName()
    val queryValue = URLEncoder.encode(getPatientIdQueryValue(), "UTF-8")

    return "$path?$queryName=$queryValue"
}

class EvaluateBuilder : WayfinderMappingBuilder("GET", getPath()) {

    init {
        requestBuilder
            .andHeader("NHSD-Target-Identifier", NHSD_TARGET_IDENTIFIER)
            .andQueryParameter(URLEncoder.encode("patient:identifier", "UTF-8"), getPatientIdQueryValue())
    }

    fun returnAfterThirtySecondsForTimeout(): Mapping {
        return respondWith(HttpStatus.SC_OK){
            andDelay(TIMEOUT_DELAY_IN_MILLISECONDS)
        }
    }

    fun returnInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR){}
    }

    fun returnNoReferralsOrUpcomingAppointments(): Mapping {
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

    fun returnReferralsAndUpcomingAppointmentsWithPartialError(): Mapping {
        val response = """
        {
            "resourceType": "Bundle",
            "type": "collection",
            "entry" : [{
                "fullUrl": "https://servita-sandbox.co.uk/CarePlan/1",
                "search": {
                    "mode": "match"
                },
                "resource": {
            "resourceType": "CarePlan",
            "status": "active",
            "intent": "order",
            "subject": {
                "identifier": {
                    "system": "https://fhir.nhs.uk/Id/nhs-number",
                    "value": "1111111111"
                }
            },
            "activity": [
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "150059575801"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575801"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "inReview"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "extension": [
                                {
                                    "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                                    "valueDate": "2125-04-06"
                                }
                            ],
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Mahogany GP Surgery"
                            }
                        ],
                        "description": "Cardiology"
                    }
                },
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "150059575802"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575802"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "bookableWasCancelled"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Mahogany GP Surgery"
                            }
                        ],
                        "description": "Cardiology"
                    }
                },
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "160023091234"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=160023091234"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "bookableWasCancelled"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Chestnut GP Surgery"
                            }
                        ]
                    }
                },
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "150059575803"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/ers/referrals?reference=150059575803"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "bookable"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "extension": [
                                {
                                    "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                                    "valueDate": "2022-04-06"
                                }
                            ],
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Mahogany GP Surgery"
                            }
                        ],
                        "description": "Cardiology"
                    }
                },
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "150059575804"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575804"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "inReview"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "extension": [
                                {
                                    "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                                    "valueDate": "2022-04-01"
                                }
                            ],
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Mahogany GP Surgery"
                            }
                        ],
                        "description": "Cardiology"
                    }
                },
                {
                    "reference": {
                        "type": "ServiceRequest",
                        "identifier": {
                            "system": "https://fhir.nhs.uk/Id/UBRN",
                            "value": "261160686915"
                        }
                    },
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=261160686915"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                "valueCoding": {
                                    "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                    "code": "inReview"
                                }
                            }
                        ],
                        "kind": "ServiceRequest",
                        "scheduledPeriod": {
                            "start": "2022-03-20T12:18:10.0150205+00:00"
                        },
                        "performer": [
                            {
                                "type": "Organization",
                                "display": "Oak GP Surgery"
                            }
                        ]
                    }
                },
                {
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "PKB"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                                "valueCoding": {
                                    "system": "http://hl7.org/fhir/appointmentstatus",
                                    "code": "booked"
                                }
                            }
                        ],
                        "kind": "Appointment",
                        "description": "General, The Willows, Croydon University Hospital, RJ6 5EU"
                    }
                },
                {
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "PKB"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                                "valueCoding": {
                                    "system": "http://hl7.org/fhir/appointmentstatus",
                                    "code": "booked"
                                }
                            }
                        ],
                        "kind": "Appointment",
                        "scheduledPeriod": {
                            "start": "2022-04-20T12:18:10.0150205+00:00"
                        },
                        "description": "Neurology, The Willows, Croydon University Hospital, RJ6 5EU"
                    }
                },
                {
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=100130035005"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                                "valueCoding": {
                                    "system": "http://hl7.org/fhir/appointmentstatus",
                                    "code": "booked"
                                }
                            }
                        ],
                        "kind": "Appointment",
                        "description": "General, The Oaks, Wembley University Hospital, WJ7 6EU"
                    }
                },
                {
                    "detail": {
                        "extension": [
                            {
                                "extension": [
                                    {
                                        "url": "client-id",
                                        "valueCode": "eRS"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=200240046006"
                            },
                            {
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                                "valueCoding": {
                                    "system": "http://hl7.org/fhir/appointmentstatus",
                                    "code": "booked"
                                }
                            }
                        ],
                        "kind": "Appointment",
                        "description": "Cardiology, The Oaks, Wembley University Hospital, WJ7 6EU"
                    }
                }
            ]
        }
    }, 
    {
        "fullUrl": "https://servita-sandbox.co.uk/OperationOutcome/1",
        "search": {
            "mode": "include"
        },
        "resource": {
            "resourceType": "OperationOutcome",
            "issue": [{
                    "severity": "error",
                    "code": "timeout",
                    "extension": [{
                            "url": "https://fhir.nhs.uk/StructureDefinition/ExtensionErrorSource",
                            "valueCode": "{client-id}"
                        }],
                    "diagnostics": "Patient Knows Best request timed out"
                }]
            }
        }]
    }
    """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }

    fun returnReferralsAndUpcomingAppointmentsUnderAgeError(): Mapping {
        val response = """
            {
              "resourceType": "OperationOutcome",
              "issue": [
                {
                  "severity": "error",
                  "code": "forbidden",
                  "diagnostics": "UNDER_16_DENIED"
                }
              ]
            }
        """

        return respondWith(HttpStatus.SC_FORBIDDEN) {
            andJsonBody(response)
                .build()
        }
    }

    fun returnReferralsAndNoUpcomingAppointments(): Mapping {
        val response = """
        {
            "resourceType": "Bundle",
            "entry": [
                {
                    "fullUrl": "https://servita-sandbox.co.uk/CarePlan/1",
                    "resource": {
                        "resourceType": "CarePlan",
                        "status": "active",
                        "intent": "order",
                        "subject": {
                            "identifier": {
                                "system": "https://fhir.nhs.uk/Id/nhs-number",
                                "value": "9659979258"
                            }
                        },
                        "activity": [
                            {
                                "reference": {
                                    "type": "ServiceRequest",
                                    "identifier": {
                                        "system": "https://fhir.nhs.uk/Id/UBRN",
                                        "value": "935345565437"
                                    }
                                },
                                "detail": {
                                    "extension": [
                                        {
                                            "extension": [
                                                {
                                                    "url": "client-id",
                                                    "valueCode": "eRS"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=935345565437"
                                        },
                                        {
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                            "valueCoding": {
                                                "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                                "code": "bookableWasCancelled"
                                            }
                                        }
                                    ],
                                    "kind": "ServiceRequest",
                                    "scheduledPeriod": {
                                        "start": "2022-04-26T14:23:09+00:00"
                                    },
                                    "performer": [
                                        {
                                            "type": "Organization",
                                            "display": "Birch GP Surgery"
                                        }
                                    ],
                                    "description": "Neurology"
                                }
                            }
                        ]
                    },
                    "search": {
                        "mode": "match"
                    }
                }
            ]
        }
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }


    fun returnReferralsAndUpcomingAppointments(): Mapping {
        val response = """
        {
          "resourceType": "Bundle",
          "entry": [
            {
              "fullUrl": "https://servita-sandbox.co.uk/CarePlan/1",
              "resource": {
                "resourceType": "CarePlan",
                "status": "active",
                "intent": "order",
                "subject": {
                  "identifier": {
                    "system": "https://fhir.nhs.uk/Id/nhs-number",
                    "value": "1111111111"
                  }
                },
                "activity": [
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "150059575801"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575801"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "inReview"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "extension": [
                          {
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                            "valueDate": "2125-04-06"
                          }
                        ],
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Mahogany GP Surgery"
                        }
                      ],
                      "description": "Cardiology"
                    }
                  },
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "150059575802"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575802"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "bookableWasCancelled"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Mahogany GP Surgery"
                        }
                      ],
                      "description": "Cardiology"
                    }
                  },
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "160023091234"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=160023091234"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "bookableWasCancelled"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Chestnut GP Surgery"
                        }
                      ]
                    }
                  },
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "150059575803"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575803"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "bookable"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "extension": [
                          {
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                            "valueDate": "2022-04-06"
                          }
                        ],
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Mahogany GP Surgery"
                        }
                      ],
                      "description": "Cardiology"
                    }
                  },
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "150059575804"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=150059575804"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "inReview"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "extension": [
                          {
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                            "valueDate": "2022-04-01"
                          }
                        ],
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Mahogany GP Surgery"
                        }
                      ],
                      "description": "Cardiology"
                    }
                  },
                  {
                    "reference": {
                      "type": "ServiceRequest",
                      "identifier": {
                        "system": "https://fhir.nhs.uk/Id/UBRN",
                        "value": "261160686915"
                      }
                    },
                    "detail": {
                      "extension": [
                        {
                            "extension": [
                                {
                                    "url": "client-id",
                                    "valueCode": "eRS"
                                }
                            ],
                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=261160686915"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                          "valueCoding": {
                            "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                            "code": "inReview"
                          }
                        }
                      ],
                      "kind": "ServiceRequest",
                      "scheduledPeriod": {
                        "start": "2022-03-20T12:18:10.0150205+00:00"
                      },
                      "performer": [
                        {
                          "type": "Organization",
                          "display": "Oak GP Surgery"
                        }
                      ]
                    }
                  },
                  {
                    "detail": {
                      "extension": [
                        {
                          "extension": [
                            {
                              "url": "client-id",
                              "valueCode": "PKB"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                          "valueCoding": {
                            "system": "http://hl7.org/fhir/appointmentstatus",
                            "code": "booked"
                          }
                        }
                      ],
                      "kind": "Appointment",
                      "description": "General, The Willows, Croydon University Hospital, RJ6 5EU"
                    }
                  },
                  {
                    "detail": {
                      "extension": [
                        {
                          "extension": [
                            {
                              "url": "client-id",
                              "valueCode": "PKB"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://stubs.local.bitraft.io:8080/pkb/upcoming-appointments?reference=932034686639"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                          "valueCoding": {
                            "system": "http://hl7.org/fhir/appointmentstatus",
                            "code": "booked"
                          }
                        }
                      ],
                      "kind": "Appointment",
                      "scheduledPeriod": {
                        "start": "2022-04-20T12:18:10.0150205+00:00"
                      },
                      "description": "Neurology, The Willows, Croydon University Hospital, RJ6 5EU"
                    }
                  },
                  {
                    "detail": {
                      "extension": [
                        {
                          "extension": [
                            {
                              "url": "client-id",
                              "valueCode": "eRS"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=100130035005"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                          "valueCoding": {
                            "system": "http://hl7.org/fhir/appointmentstatus",
                            "code": "booked"
                          }
                        }
                      ],
                      "kind": "Appointment",
                      "description": "General, The Oaks, Wembley University Hospital, WJ7 6EU"
                    }
                  },
                  {
                    "detail": {
                      "extension": [
                        {
                          "extension": [
                            {
                              "url": "client-id",
                              "valueCode": "eRS"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=200240046006"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                          "valueCoding": {
                            "system": "http://hl7.org/fhir/appointmentstatus",
                            "code": "booked"
                          }
                        }
                      ],
                      "kind": "Appointment",
                      "description": "Cardiology, The Oaks, Wembley University Hospital, WJ7 6EU"
                    }
                  },
                  {
                    "detail": {
                      "extension": [
                        {
                          "extension": [
                            {
                              "url": "client-id",
                              "valueCode": "PKB"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://stubs.local.bitraft.io:8080/pkb/upcoming-appointments?reference=932034686639"
                        },
                        {
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                          "valueCoding": {
                            "system": "http://hl7.org/fhir/appointmentstatus",
                            "code": "cancelled"
                          }
                        }
                      ],
                      "kind": "Appointment",
                      "scheduledPeriod": {
                        "start": "2300-04-20T12:18:10.0150205+00:00"
                      },
                      "description": "Neurology, The Willows, Croydon University Hospital, RJ6 5EU"
                    }
                  }
                ]
              }
            }
          ]
        }
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }

    fun returnReferralsAndUpcomingAppointmentsForProvider(provider: String): Mapping {
        val deepLink = when(provider){
            "AccurxWayfinder" -> "http://accurxwayfinder.stubs.local.bitraft.io:8080/api/OpenIdConnect/AuthenticateManageAppointment?" +
                "appointmentToken=XXXX"
            "DrDoctor" -> "http://drdoctor.stubs.local.bitraft.io:8080/appointments/" +
                "123456?from=nhsApp"
            "HealthcareComms" -> "http://hcc.stubs.local.bitraft.io:8080/appointments/987654321"            
            "PKB" -> "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?" +
                "phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e"
            "Netcall" -> "http://netcall.stubs.local.bitraft.io:8080/Appointments?" +
                 "id=49482520-9026-4398-b3a0-2b738ebc1365&trust=789"
            "Zesty" -> "http://zesty.stubs.local.bitraft.io:8080/nhs/origin_appointment?" +
                "resource_id=XXXX"
            else -> "http://stubs.local.bitraft.io:8080"
        }

        val response = """
        {
            "resourceType": "Bundle",
            "entry": [
                {
                    "fullUrl": "https://servita-sandbox.co.uk/CarePlan/1",
                    "resource": {
                        "resourceType": "CarePlan",
                        "status": "active",
                        "intent": "order",
                        "subject": {
                            "identifier": {
                                "system": "https://fhir.nhs.uk/Id/nhs-number",
                                "value": "1111111111"
                            }
                        },
                        "activity": [
                            {
                                "reference": {
                                    "type": "ServiceRequest",
                                    "identifier": {
                                        "system": "https://fhir.nhs.uk/Id/UBRN",
                                        "value": "150059575801"
                                    }
                                },
                                "detail": {
                                    "extension": [
                                        {
                                            "extension": [
                                                {
                                                    "url": "client-id",
                                                    "valueCode": "$provider"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "$deepLink"
                                        },
                                        {
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
                                            "valueCoding": {
                                                "system": "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                                                "code": "inReview"
                                            }
                                        }
                                    ],
                                    "kind": "ServiceRequest",
                                    "scheduledPeriod": {
                                        "extension": [
                                            {
                                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
                                                "valueDate": "2125-04-06"
                                            }
                                        ],
                                        "start": "2022-03-20T12:18:10.0150205+00:00"
                                    },
                                    "performer": [
                                        {
                                            "type": "Organization",
                                            "display": "Mahogany GP Surgery"
                                        }
                                    ],
                                    "description": "Cardiology"
                                }
                            },
                            {
                                "detail": {
                                    "extension": [
                                        {
                                            "extension": [
                                                {
                                                    "url": "client-id",
                                                    "valueCode": "$provider"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "$deepLink"
                                        },
                                        {
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
                                            "valueCoding": {
                                                "system": "http://hl7.org/fhir/appointmentstatus",
                                                "code": "booked"
                                            }
                                        }
                                    ],
                                    "kind": "Appointment",
                                    "description": "General, The Willows, Croydon University Hospital, RJ6 5EU"
                                }
                            }
                        ]
                    }
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
