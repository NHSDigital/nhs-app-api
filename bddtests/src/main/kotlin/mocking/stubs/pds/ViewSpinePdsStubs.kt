package mocking.stubs.pds

import com.github.tomakehurst.wiremock.stubbing.Scenario
import mocking.MockingClient
import mocking.spine.pds.PdsNominatedPharmacyBuilder

@Suppress("MaxLineLength")
class ViewSpinePdsStubs(private val mockingClient: MockingClient) {

    companion object {
        private const val IP = "10.0.225.22"
        private const val fromAsid = "918999199235"
        private const val toAsid = "200000000627"
    }

    fun generateSpineStubs() {
        val changeNominatedPharmacy = "Pharmacy"
        val pharmacyUpdatedScenario = "CHANGED"
        val updateSoapAction = "urn:nhs:names:services:pdsquery/QUPA_IN000008UK02"

        // Get nominated pharmacy ods code
        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder(updateSoapAction)
                    .respondWithSuccess(getP1Response("FAJ15"))
                    .inScenario(changeNominatedPharmacy)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(pharmacyUpdatedScenario) }

        // Get nominated pharmacy ods code (2nd Time after update )
        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder(updateSoapAction)
                    .respondWithSuccess(getP1Response("FK275"))
                    .inScenario(changeNominatedPharmacy)
                    .whenScenarioStateIs(pharmacyUpdatedScenario)
        }

        // Update nominated pharmacy ods code
        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pds/PRPA_IN000203UK06")
                    .respondWithAccepted() }

    }

    fun getP1Response(odsCode: String): String {
        return """<?xml version='1.0' encoding='UTF-8'?>
            <SOAP-ENV:Envelope
	        xmlns:crs="http://national.carerecords.nhs.uk/schema/crs/"
	        xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
	xmlns:wsa="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns="urn:hl7-org:v3"
	xmlns:hl7="urn:hl7-org:v3">
	<SOAP-ENV:Header>
		<wsa:MessageID>uuid:579F50BC-3C11-11E9-AE9C-6C3BE5A861CE</wsa:MessageID>
		<wsa:Action>urn:nhs:names:services:pdsquery/QUPA_IN000009UK03</wsa:Action>
		<wsa:To>${IP}</wsa:To>
		<wsa:From>
			<wsa:Address>https://192.168.128.11/syncservice-pds/pds</wsa:Address>
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
		<wsa:RelatesTo>uuid:8071319F-E1D2-4DA6-9649-96A3FA3499F8</wsa:RelatesTo>
	</SOAP-ENV:Header>
	<SOAP-ENV:Body>
		<retrievalQueryResponse>
			<QUPA_IN000009UK03>
				<id root="579F50BC-3C11-11E9-AE9C-6C3BE5A861CE"/>
				<creationTime value="20190301110119"/>
				<versionCode code="3NPfIT6.3.01"/>
				<interactionId root="2.16.840.1.113883.2.1.3.2.4.12" extension="QUPA_IN000009UK03"/>
				<processingCode code="P"/>
				<processingModeCode code="T"/>
				<acceptAckCode code="NE"/>
				<acknowledgement typeCode="AA">
					<messageRef>
						<id root="8071319F-E1D2-4DA6-9649-96A3FA3499F8"/>
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
								<id root="1.2.826.0.1285.0.2.0.107" extension="${toAsid}"/>
							</agentSystemSDS>
						</AgentSystemSDS>
					</author1>
					<subject typeCode="SUBJ">
						<PDSResponse
							xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" classCode="OBS" moodCode="EVN">
							<pertinentInformation typeCode="PERT">
								<pertinentSerialChangeNumber classCode="OBS" moodCode="EVN">
									<code code="2" codeSystem="2.16.840.1.113883.2.1.3.2.4.17.35"/>
									<value value="1"/>
								</pertinentSerialChangeNumber>
							</pertinentInformation>
							<subject typeCode="SBJ">
								<patientRole classCode="PAT">
									<id root="2.16.840.1.113883.2.1.4.1" extension="9449306680"/>
									<patientPerson classCode="PSN" determinerCode="INSTANCE">
										<administrativeGenderCode code="1"/>
										<birthTime value="20110614"/>
										<playedOtherProviderPatient classCode="PAT">
                                            <subjectOf typeCode="SBJ">
                                                <patientCareProvisionEvent classCode="PCPR" moodCode="EVN">
                                                    <code codeSystem="2.16.840.1.113883.2.1.3.2.4.17.37" code="P1"/>
                                                    <effectiveTime>
                                                        <low value="20170814"/>
                                                    </effectiveTime>
                                                    <id root="2.16.840.1.113883.2.1.3.2.4.18.1" extension="EA42CF39"/>
                                                    <performer typeCode="PRF">
                                                        <assignedEntity classCode="ASSIGNED">
                                                            <id root="2.16.840.1.113883.2.1.4.3"
                                                             extension="${odsCode}"/>
                                                        </assignedEntity>
                                                    </performer>
                                                </patientCareProvisionEvent>
                                            </subjectOf>
                                    </playedOtherProviderPatient>
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
                """
    }
}