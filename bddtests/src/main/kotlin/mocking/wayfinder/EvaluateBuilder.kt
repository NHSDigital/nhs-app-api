@file:Suppress("MaxLineLength")
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
                                        "url": "code",
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
                                    "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                        "url": "code",
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
                                        "url": "code",
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
                                        "url": "code",
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
                                    "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                        "url": "code",
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
                                    "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                        "url": "code",
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
                                        "url": "code",
                                        "valueCode": "PKB"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?uniqueId=8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e&contextUserId=27911"
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
                                        "url": "code",
                                        "valueCode": "PKB"
                                    }
                                ],
                                "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?uniqueId=8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e&contextUserId=27911"
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
                                        "url": "code",
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
                                        "url": "code",
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
               "resourceType":"Bundle",
               "entry":[
                  {
                     "fullUrl":"https://servita-sandbox.co.uk/CarePlan/1",
                     "resource":{
                        "resourceType":"CarePlan",
                        "status":"active",
                        "intent":"order",
                        "subject":{
                           "identifier":{
                              "system":"https://fhir.nhs.uk/Id/nhs-number",
                              "value":"9290220899"
                           }
                        }
                     },
                     "search":{
                        "mode":"match"
                     }
                  },
                  {
                     "fullUrl":"https://servita-sandbox.co.uk/OperationOutCome/1",
                     "resource":{
                        "resourceType":"OperationOutcome",
                        "issue":[
                           {
                              "extension":[
                                 {
                                    "url":"https://fhir.nhs.uk/StructureDefinition/ExtensionErrorSource",
                                    "valueCode":"company-4"
                                 }
                              ],
                              "severity":"error",
                              "code":"forbidden",
                              "diagnostics":"UNDER_16_DENIED"
                           }
                        ]
                     },
                     "search":{
                        "mode":"include"
                     }
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
                                                    "url": "code",
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
                                    "url": "code",
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
                            "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                    "url": "code",
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
                                    "url": "code",
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
                                    "url": "code",
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
                            "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                    "url": "code",
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
                            "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                    "url": "code",
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
                              "url": "code",
                              "valueCode": "PKB"
                            }
                          ],
                          "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                          "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?uniqueId=8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e&contextUserId=27911"
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
                              "url": "code",
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
                              "url": "code",
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
                              "url": "code",
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
                              "url": "code",
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

    fun returnReferralsAndUpcomingAppointmentsErs(): Mapping {
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
                                                    "url": "code",
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
                                                "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                                    "url": "code",
                                                    "valueCode": "eRS"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "http://silver.local.bitraft.io:5001/nhslogin?ubrn=821923575528"
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

    fun returnReferralsAndUpcomingAppointmentsPkb(): Mapping {
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
                                                    "url": "code",
                                                    "valueCode": "PKB"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?uniqueId=8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e&contextUserId=27911"
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
                                                "url": "https://fhir.nhs.net/ReviewDueDate",
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
                                                    "url": "code",
                                                    "valueCode": "eRS"
                                                }
                                            ],
                                            "url": "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
                                            "valueUrl": "http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?uniqueId=8b8d1edc-8cb0-49b2-8fb0-4b7ab564a67e&contextUserId=27911"
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
