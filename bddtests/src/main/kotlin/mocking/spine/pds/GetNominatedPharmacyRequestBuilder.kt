package mocking.spine.pds

import java.lang.StringBuilder

class GetNominatedPharmacyRequestBuilder
{
    companion object {
        private const val IP = "10.0.225.22"
        private const val fromAsid = "200000000355"
        private const val toAsid = "200000000355"

        fun getResponse(nhsNumber: String, surname: String, dateOfBirth: String): String {
            return """
            <?xml version='1.0' encoding='UTF-8'?>
            <SOAP-ENV:Envelope
              xmlns:crs="http://national.carerecords.nhs.uk/schema/crs/"
              xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
              xmlns:wsa="http://schemas.xmlsoap.org/ws/2004/08/addressing"
              xmlns="urn:hl7-org:v3"
              xmlns:hl7="urn:hl7-org:v3">
              <SOAP-ENV:Header>
                <wsa:MessageID>uuid:7C24CDCA-5C5C-11E9-B0FB-F4034348F02E</wsa:MessageID>
                <wsa:Action>urn:nhs:names:services:pdsquery/QUPA_IN000009UK03</wsa:Action>
                <wsa:To>${IP}</wsa:To>
                <wsa:From>
                  <wsa:Address>https://192.168.128.11/sync-service</wsa:Address>
                </wsa:From>
                <communicationFunctionRcv typeCode="RCV">
                  <device classCode="DEV" determinerCode="INSTANCE">
                    <id root="1.2.826.0.1285.0.2.0.107" extension="${fromAsid}"/>
                  </device>
                </communicationFunctionRcv>
                <communicationFunctionSnd typeCode="SND">
                  <device classCode="DEV" determinerCode="INSTANCE">
                    <id root="1.2.826.0.1285.0.2.0.107" extension="${toAsid}"/>
                  </device>
                </communicationFunctionSnd>
                <wsa:RelatesTo>uuid:289852c1-b232-4ae2-b6c8-f3556e44e2b8</wsa:RelatesTo>
              </SOAP-ENV:Header>
              <SOAP-ENV:Body>
                <retrievalQueryResponse>
                  <QUPA_IN000009UK03>
                    <id root="7C24CDCA-5C5C-11E9-B0FB-F4034348F02E"/>
                    <creationTime value="20190411131950"/>
                    <versionCode code="3NPfIT6.3.01"/>
                    <interactionId root="2.16.840.1.113883.2.1.3.2.4.12" extension="QUPA_IN000009UK03"/>
                    <processingCode code="P"/>
                    <processingModeCode code="T"/>
                    <acceptAckCode code="NE"/>
                    <acknowledgement typeCode="AA">
                      <messageRef>
                        <id root="289852c1-b232-4ae2-b6c8-f3556e44e2b8"/>
                      </messageRef>
                    </acknowledgement>
                    <communicationFunctionRcv typeCode="RCV">
                      <device classCode="DEV" determinerCode="INSTANCE">
                        <id root="1.2.826.0.1285.0.2.0.107" extension="${fromAsid}"/>
                      </device>
                    </communicationFunctionRcv>
                    <communicationFunctionSnd typeCode="SND">
                      <device classCode="DEV" determinerCode="INSTANCE">
                        <id root="1.2.826.0.1285.0.2.0.107" extension="${toAsid}"/>
                      </device>
                    </communicationFunctionSnd>
                    <ControlActEvent classCode="CACT" moodCode="EVN">
                      <author1 typeCode="AUT">
                        <AgentSystemSDS classCode="AGNT">
                          <agentSystemSDS classCode="DEV" determinerCode="INSTANCE">
                            <id root="1.2.826.0.1285.0.2.0.107" extension="200000000355"/>
                          </agentSystemSDS>
                        </AgentSystemSDS>
                      </author1>
                      <subject typeCode="SUBJ">
                        <PDSResponse
                          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" classCode="OBS" moodCode="EVN">
                          <pertinentInformation typeCode="PERT">
                            <pertinentSerialChangeNumber classCode="OBS" moodCode="EVN">
                              <code code="2" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.35"/>
                              <value value="2"/>
                            </pertinentSerialChangeNumber>
                          </pertinentInformation>
                          <subject typeCode="SBJ">
                            <patientRole classCode="PAT">
                              <id root="2.16.840.1.113883.2.1.4.1" extension="${nhsNumber}"/>
                              <patientPerson classCode="PSN" determinerCode="INSTANCE">
                                <administrativeGenderCode code="1"/>
                                <birthTime value="${dateOfBirth}"/>
                                <playedOtherProviderPatient classCode="PAT">
                                  <subjectOf typeCode="SBJ">
                                    <patientCareProvisionEvent classCode="PCPR" moodCode="EVN">
                                      <code codeSystem="2.16.840.1.113883.2.1.3.2.4.17.37" code="1"/>
                                      <effectiveTime>
                                        <low value="19630422"/>
                                      </effectiveTime>
                                      <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="HFmjd"/>
                                      <performer typeCode="PRF">
                                        <assignedEntity classCode="ASSIGNED">
                                          <id root="2.16.840.1.113883.2.1.4.3" extension="E87021"/>
                                        </assignedEntity>
                                      </performer>
                                    </patientCareProvisionEvent>
                                  </subjectOf>
                                </playedOtherProviderPatient>
                                <COCT_MT000201UK02.PartOfWhole classCode="PART">
                                  <addr use="H">
                                    <streetAddressLine>SUFFOLK HOUSE</streetAddressLine>
                                    <streetAddressLine>COCHRANE MEWS</streetAddressLine>
                                    <streetAddressLine/>
                                    <streetAddressLine>LONDON</streetAddressLine>
                                    <streetAddressLine/>
                                    <postalCode>NW8 6PA</postalCode>
                                    <useablePeriod>
                                      <low value="20120913"/>
                                    </useablePeriod>
                                    <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="EwEOT"/>
                                  </addr>
                                </COCT_MT000201UK02.PartOfWhole>
                                <COCT_MT000203UK02.PartOfWhole classCode="PART">
                                  <partPerson classCode="PSN" determinerCode="INSTANCE">
                                    <name use="L">
                                      <prefix>MR</prefix>
                                      <given>Roland</given>
                                      <given>Lionel</given>
                                      <family>${surname}</family>
                                      <validTime>
                                        <low value="20090308"/>
                                      </validTime>
                                      <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="Jpggc"/>
                                    </name>
                                  </partPerson>
                                </COCT_MT000203UK02.PartOfWhole>
                              </patientPerson>
                              <subjectOf8 typeCode="SBJ">
                                <previousNhsContact classCode="OBS" moodCode="EVN">
                                  <code code="17" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.35"/>
                                  <value codeSystem="2.16.840.1.113883.2.1.3.2.4.17.38" code="0"/>
                                </previousNhsContact>
                              </subjectOf8>
                            </patientRole>
                          </subject>
                        </PDSResponse>
                      </subject>
                      <queryAck type="QueryAck">
                        <queryResponseCode code="OK"/>
                      </queryAck>
                    </ControlActEvent>
                  </QUPA_IN000009UK03>
                </retrievalQueryResponse>
              </SOAP-ENV:Body>
            </SOAP-ENV:Envelope>
        """.trimIndent()
        }

        fun getResponse(personalCheckDetails: PersonalCheckDetails,
                        odsCode: String, pharmacyTypes: kotlin.Array<String>, code : String? = null): String {
            return """
            <?xml version='1.0' encoding='UTF-8'?>
            <SOAP-ENV:Envelope
              xmlns:crs="http://national.carerecords.nhs.uk/schema/crs/"
              xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
              xmlns:wsa="http://schemas.xmlsoap.org/ws/2004/08/addressing"
              xmlns="urn:hl7-org:v3"
              xmlns:hl7="urn:hl7-org:v3">
              <SOAP-ENV:Header>
                <wsa:MessageID>uuid:7C24CDCA-5C5C-11E9-B0FB-F4034348F02E</wsa:MessageID>
                <wsa:Action>urn:nhs:names:services:pdsquery/QUPA_IN000009UK03</wsa:Action>
                <wsa:To>${IP}</wsa:To>
                <wsa:From>
                  <wsa:Address>https://192.168.128.11/sync-service</wsa:Address>
                </wsa:From>
                <communicationFunctionRcv typeCode="RCV">
                  <device classCode="DEV" determinerCode="INSTANCE">
                    <id root="1.2.826.0.1285.0.2.0.107" extension="${fromAsid}"/>
                  </device>
                </communicationFunctionRcv>
                <communicationFunctionSnd typeCode="SND">
                  <device classCode="DEV" determinerCode="INSTANCE">
                    <id root="1.2.826.0.1285.0.2.0.107" extension="${toAsid}"/>
                  </device>
                </communicationFunctionSnd>
                <wsa:RelatesTo>uuid:289852c1-b232-4ae2-b6c8-f3556e44e2b8</wsa:RelatesTo>
              </SOAP-ENV:Header>
              <SOAP-ENV:Body>
                <retrievalQueryResponse>
                  <QUPA_IN000009UK03>
                    <id root="7C24CDCA-5C5C-11E9-B0FB-F4034348F02E"/>
                    <creationTime value="20190411131950"/>
                    <versionCode code="3NPfIT6.3.01"/>
                    <interactionId root="2.16.840.1.113883.2.1.3.2.4.12" extension="QUPA_IN000009UK03"/>
                    <processingCode code="P"/>
                    <processingModeCode code="T"/>
                    <acceptAckCode code="NE"/>
                    <acknowledgement typeCode="AA">
                      <messageRef>
                        <id root="289852c1-b232-4ae2-b6c8-f3556e44e2b8"/>
                      </messageRef>
                    </acknowledgement>
                    <communicationFunctionRcv typeCode="RCV">
                      <device classCode="DEV" determinerCode="INSTANCE">
                        <id root="1.2.826.0.1285.0.2.0.107" extension="${fromAsid}"/>
                      </device>
                    </communicationFunctionRcv>
                    <communicationFunctionSnd typeCode="SND">
                      <device classCode="DEV" determinerCode="INSTANCE">
                        <id root="1.2.826.0.1285.0.2.0.107" extension="${toAsid}"/>
                      </device>
                    </communicationFunctionSnd>
                    <ControlActEvent classCode="CACT" moodCode="EVN">
                      <author1 typeCode="AUT">
                        <AgentSystemSDS classCode="AGNT">
                          <agentSystemSDS classCode="DEV" determinerCode="INSTANCE">
                            <id root="1.2.826.0.1285.0.2.0.107" extension="200000000355"/>
                          </agentSystemSDS>
                        </AgentSystemSDS>
                      </author1>
                      <reason typeCode="RSON">
                        <justifyingDetectedIssueEvent classCode="ALRT" moodCode="EVN">
                          <code code="9" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.42" displayName="Success retrieval">
                            <qualifier code="WG"/>
                          </code>
                        </justifyingDetectedIssueEvent>
                      </reason>
                      <subject typeCode="SUBJ">
                        <PDSResponse
                          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" classCode="OBS" moodCode="EVN">
                          <pertinentInformation typeCode="PERT">
                            <pertinentSerialChangeNumber classCode="OBS" moodCode="EVN">
                              <code code="2" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.35"/>
                              <value value="2"/>
                            </pertinentSerialChangeNumber>
                          </pertinentInformation>
                          <subject typeCode="SBJ">
                            <patientRole classCode="PAT">
                              ${ getConfidentialityCode(code) }
                              <id root="2.16.840.1.113883.2.1.4.1" extension="${personalCheckDetails.nhsNumber}"/>
                              <patientPerson classCode="PSN" determinerCode="INSTANCE">
                                <administrativeGenderCode code="1"/>
                                <birthTime value="${personalCheckDetails.dateOfBirth}"/>
                                <playedOtherProviderPatient classCode="PAT">
                                  <subjectOf typeCode="SBJ">
                                    <patientCareProvisionEvent classCode="PCPR" moodCode="EVN">
                                      <code codeSystem="2.16.840.1.113883.2.1.3.2.4.17.37" code="1"/>
                                      <effectiveTime>
                                        <low value="19630422"/>
                                      </effectiveTime>
                                      <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="HFmjd"/>
                                      <performer typeCode="PRF">
                                        <assignedEntity classCode="ASSIGNED">
                                          <id root="2.16.840.1.113883.2.1.4.3" extension="E87021"/>
                                        </assignedEntity>
                                      </performer>
                                    </patientCareProvisionEvent>
                                  </subjectOf>
                                </playedOtherProviderPatient>
                                      ${ getPlayedOtherProviderPatient(odsCode, pharmacyTypes) }
                                <COCT_MT000201UK02.PartOfWhole classCode="PART">
                                  <addr use="H">
                                    <streetAddressLine>SUFFOLK HOUSE</streetAddressLine>
                                    <streetAddressLine>COCHRANE MEWS</streetAddressLine>
                                    <streetAddressLine/>
                                    <streetAddressLine>LONDON</streetAddressLine>
                                    <streetAddressLine/>
                                    <postalCode>NW8 6PA</postalCode>
                                    <useablePeriod>
                                      <low value="20120913"/>
                                    </useablePeriod>
                                    <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="EwEOT"/>
                                  </addr>
                                </COCT_MT000201UK02.PartOfWhole>
                                <COCT_MT000203UK02.PartOfWhole classCode="PART">
                                  <partPerson classCode="PSN" determinerCode="INSTANCE">
                                    <name use="L">
                                      <prefix>MR</prefix>
                                      <given>Roland</given>
                                      <given>Lionel</given>
                                      <family>${personalCheckDetails.surname}</family>
                                      <validTime>
                                        <low value="20090308"/>
                                      </validTime>
                                      <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="Jpggc"/>
                                    </name>
                                  </partPerson>
                                </COCT_MT000203UK02.PartOfWhole>
                              </patientPerson>
                              <subjectOf8 typeCode="SBJ">
                                <previousNhsContact classCode="OBS" moodCode="EVN">
                                  <code code="17" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.35"/>
                                  <value codeSystem="2.16.840.1.113883.2.1.3.2.4.17.38" code="0"/>
                                </previousNhsContact>
                              </subjectOf8>
                            </patientRole>
                          </subject>
                        </PDSResponse>
                      </subject>
                      <queryAck type="QueryAck">
                        <queryResponseCode code="OK"/>
                      </queryAck>
                    </ControlActEvent>
                  </QUPA_IN000009UK03>
                </retrievalQueryResponse>
              </SOAP-ENV:Body>
            </SOAP-ENV:Envelope>
        """.trimIndent()
        }

        fun getConfidentialityCode(code: String?) : StringBuilder {
            val ccBuilder = StringBuilder()
            if (code != null) {
                ccBuilder.append(
                        """<confidentialityCode codeSystem="2.16.840.1.113883.2.1.3.2.4.16.1" code="${code}"/>""")
            }
            return ccBuilder
        }

        fun getPlayedOtherProviderPatient(odsCode: String, pharmacyTypes: kotlin.Array<String>) : StringBuilder {
            val patientCareProvisionBuilder = StringBuilder()
            if(pharmacyTypes.size > 0) {
                for (pharmacyType in pharmacyTypes) {
                    patientCareProvisionBuilder.append("""<playedOtherProviderPatient classCode="PAT">
                                  <subjectOf typeCode="SBJ"> <patientCareProvisionEvent classCode="PCPR" moodCode="EVN">
                                        <code codeSystem="2.16.840.1.113883.2.1.3.2.4.17.37" code="${pharmacyType}"/>
                                      <effectiveTime>
                                        <low value="20190402"/>
                                      </effectiveTime>
                                      <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="P000000042"/>
                                      <performer typeCode="PRF">
                                        <assignedEntity classCode="ASSIGNED">
                                          <id root="2.16.840.1.113883.2.1.4.3" extension="${odsCode}"/>
                                        </assignedEntity>
                                      </performer>
                                        </patientCareProvisionEvent> </subjectOf>
                                </playedOtherProviderPatient>""")
                }
            }
            return patientCareProvisionBuilder
        }
    }
}