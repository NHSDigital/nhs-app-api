using System;
using System.Text;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Spine
{
    public static class SpineResponseBuilder
    {
        private const string Ip = "10.0.225.22";
        private const string FromAsid = "200000000354";
        private const string ToAsid = "200000000355";
        private const string PdsTraceAction = "urn:nhs:names:services:pdsquery/QUPA_IN000008UK02";
        private const string UpdateNominatedPharmacyAction = "PRPA_IN000203UK03";

        public const string P1PharmacyType = "P1";
        public const string NominatedPharmacyOdsCode = "FV493";

        public static bool IsNominatedPharmacyUpdateRequest(string request)
        {
            return request.Contains(UpdateNominatedPharmacyAction, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart, StringComparison.Ordinal) && strSource.Contains(strEnd, StringComparison.Ordinal))
            {
                var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
                return strSource.Substring(start, end - start);
            }
            else
            {
                return "";
            }
        }

        public static string GetPdsTraceResponse(Patient patient, bool hasNominatedPharmacy, string pharmacyType)
        {
            return GetPdsTraceResponse(patient, hasNominatedPharmacy, new[] {pharmacyType});
        }

        private static string GetPdsTraceResponse(Patient patient, bool hasNominatedPharmacy, string[] pharmacyTypes)
        {
            return
                $@"<?xml version='1.0' encoding='UTF-8'?>
            <SOAP-ENV:Envelope
              xmlns:crs=""http://national.carerecords.nhs.uk/schema/crs/""
              xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""
              xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing""
              xmlns=""urn:hl7-org:v3""
              xmlns:hl7=""urn:hl7-org:v3"">
              <SOAP-ENV:Header>
                <wsa:MessageID>uuid:7C24CDCA-5C5C-11E9-B0FB-F4034348F02E</wsa:MessageID>
                <wsa:Action>{PdsTraceAction}</wsa:Action>
                <wsa:To>{Ip}</wsa:To>
                <wsa:From>
                  <wsa:Address>https://192.168.128.11/sync-service</wsa:Address>
                </wsa:From>
                <communicationFunctionRcv typeCode=""RCV"">
                  <device classCode=""DEV"" determinerCode=""INSTANCE"">
                    <id root=""1.2.826.0.1285.0.2.0.107"" extension=""{FromAsid}""/>
                  </device>
                </communicationFunctionRcv>
                <communicationFunctionSnd typeCode=""SND"">
                  <device classCode=""DEV"" determinerCode=""INSTANCE"">
                    <id root=""1.2.826.0.1285.0.2.0.107"" extension=""{ToAsid}""/>
                  </device>
                </communicationFunctionSnd>
                <wsa:RelatesTo>uuid:289852c1-b232-4ae2-b6c8-f3556e44e2b8</wsa:RelatesTo>
              </SOAP-ENV:Header>
              <SOAP-ENV:Body>
                <retrievalQueryResponse>
                  <QUPA_IN000009UK03>
                    <id root=""7C24CDCA-5C5C-11E9-B0FB-F4034348F02E""/>
                    <creationTime value=""20190411131950""/>
                    <versionCode code=""3NPfIT6.3.01""/>
                    <interactionId root=""2.16.840.1.113883.2.1.3.2.4.12"" extension=""QUPA_IN000009UK03""/>
                    <processingCode code=""P""/>
                    <processingModeCode code=""T""/>
                    <acceptAckCode code=""NE""/>
                    <acknowledgement typeCode=""AA"">
                      <messageRef>
                        <id root=""289852c1-b232-4ae2-b6c8-f3556e44e2b8""/>
                      </messageRef>
                    </acknowledgement>
                    <communicationFunctionRcv typeCode=""RCV"">
                      <device classCode=""DEV"" determinerCode=""INSTANCE"">
                        <id root=""1.2.826.0.1285.0.2.0.107"" extension=""{FromAsid}""/>
                      </device>
                    </communicationFunctionRcv>
                    <communicationFunctionSnd typeCode=""SND"">
                      <device classCode=""DEV"" determinerCode=""INSTANCE"">
                        <id root=""1.2.826.0.1285.0.2.0.107"" extension=""{ToAsid}""/>
                      </device>
                    </communicationFunctionSnd>
                    <ControlActEvent classCode=""CACT"" moodCode=""EVN"">
                      <author1 typeCode=""AUT"">
                        <AgentSystemSDS classCode=""AGNT"">
                          <agentSystemSDS classCode=""DEV"" determinerCode=""INSTANCE"">
                            <id root=""1.2.826.0.1285.0.2.0.107"" extension=""200000000355""/>
                          </agentSystemSDS>
                        </AgentSystemSDS>
                      </author1>
                      <subject typeCode=""SUBJ"">
                        <PDSResponse
                          xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" classCode=""OBS"" moodCode=""EVN"">
                          <pertinentInformation typeCode=""PERT"">
                            <pertinentSerialChangeNumber classCode=""OBS"" moodCode=""EVN"">
                              <code code=""2"" codeSystem=""2.16.840.1.113883.2.1.3.2.4.17.35""/>
                              <value value=""2""/>
                            </pertinentSerialChangeNumber>
                          </pertinentInformation>
                          <subject typeCode=""SBJ"">
                            <patientRole classCode=""PAT"">
                              <id root=""2.16.840.1.113883.2.1.4.1"" extension=""{patient.NhsNumber.StringValue}""/>
                              <patientPerson classCode=""PSN"" determinerCode=""INSTANCE"">
                                <administrativeGenderCode code=""1""/>
                                <birthTime value=""19800203""/>
                                <playedOtherProviderPatient classCode=""PAT"">
                                  <subjectOf typeCode=""SBJ"">
                                    <patientCareProvisionEvent classCode=""PCPR"" moodCode=""EVN"">
                                      <code codeSystem=""2.16.840.1.113883.2.1.3.2.4.17.37"" code=""1""/>
                                      <effectiveTime>
                                        <low value=""19630422""/>
                                      </effectiveTime>
                                      <id root=""2.16.840.1.113883.2.1.3.2.4.18.1"" extension=""HFmjd""/>
                                      <performer typeCode=""PRF"">
                                        <assignedEntity classCode=""ASSIGNED"">
                                          <id root=""2.16.840.1.113883.2.1.4.3"" extension=""E87021""/>
                                        </assignedEntity>
                                      </performer>
                                    </patientCareProvisionEvent>
                                  </subjectOf>
                                </playedOtherProviderPatient>
                                      {(hasNominatedPharmacy ? GetPlayedOtherProviderPatient(pharmacyTypes) : string.Empty)}
                                <COCT_MT000201UK02.PartOfWhole classCode=""PART"">
                                  <addr use=""H"">
                                    <streetAddressLine>SUFFOLK HOUSE</streetAddressLine>
                                    <streetAddressLine>COCHRANE MEWS</streetAddressLine>
                                    <streetAddressLine/>
                                    <streetAddressLine>LONDON</streetAddressLine>
                                    <streetAddressLine/>
                                    <postalCode>NW8 6PA</postalCode>
                                    <useablePeriod>
                                      <low value=""20120913""/>
                                    </useablePeriod>
                                    <id root=""2.16.840.1.113883.2.1.3.2.4.18.1"" extension=""EwEOT""/>
                                  </addr>
                                </COCT_MT000201UK02.PartOfWhole>
                                <COCT_MT000203UK02.PartOfWhole classCode=""PART"">
                                  <partPerson classCode=""PSN"" determinerCode=""INSTANCE"">
                                    <name use=""L"">
                                      <prefix>{patient.PersonalDetails.Name.Title}</prefix>
                                      <given>{patient.PersonalDetails.Name.GivenName}</given>
                                      <family>{patient.NhsNumber.StringValue}</family>
                                      <validTime>
                                        <low value=""20090308""/>
                                      </validTime>
                                      <id root=""2.16.840.1.113883.2.1.3.2.4.18.1"" extension=""Jpggc""/>
                                    </name>
                                  </partPerson>
                                </COCT_MT000203UK02.PartOfWhole>
                              </patientPerson>
                              <subjectOf8 typeCode=""SBJ"">
                                <previousNhsContact classCode=""OBS"" moodCode=""EVN"">
                                  <code code=""17"" codeSystem=""2.16.840.1.113883.2.1.3.2.4.17.35""/>
                                  <value codeSystem=""2.16.840.1.113883.2.1.3.2.4.17.38"" code=""0""/>
                                </previousNhsContact>
                              </subjectOf8>
                            </patientRole>
                          </subject>
                        </PDSResponse>
                      </subject>
                      <queryAck type=""QueryAck"">
                        <queryResponseCode code=""OK""/>
                      </queryAck>
                    </ControlActEvent>
                  </QUPA_IN000009UK03>
                </retrievalQueryResponse>
              </SOAP-ENV:Body>
            </SOAP-ENV:Envelope>";
        }

        private static string GetPlayedOtherProviderPatient(string[] pharmacyTypes)
        {
            StringBuilder patientCareProvisionBuilder = new StringBuilder();

            if (pharmacyTypes.Length > 0)
            {
                foreach (var pharmacyType in pharmacyTypes)
                {
                    var pharmacyTypeString =
                        $@"<playedOtherProviderPatient classCode=""PAT"">
                  <subjectOf typeCode=""SBJ"">
                    <patientCareProvisionEvent classCode=""PCPR"" moodCode=""EVN"">
                      <code codeSystem=""2.16.840.1.113883.2.1.3.2.4.17.37"" code=""{pharmacyType}""/>
                      <effectiveTime>
                        <low value=""20190402""/>
                      </effectiveTime>
                      <id root=""2.16.840.1.113883.2.1.3.2.4.18.1"" extension=""P000000042""/>
                      <performer typeCode=""PRF"">
                        <assignedEntity classCode=""ASSIGNED"">
                          <id root=""2.16.840.1.113883.2.1.4.3"" extension=""{NominatedPharmacyOdsCode}""/>
                        </assignedEntity>
                      </performer>
                    </patientCareProvisionEvent>
                  </subjectOf>
                </playedOtherProviderPatient>";

                    patientCareProvisionBuilder.Append(pharmacyTypeString);
                }
            }

            return patientCareProvisionBuilder.ToString();
        }
    }
}