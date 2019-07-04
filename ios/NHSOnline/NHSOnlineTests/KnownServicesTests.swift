import XCTest
@testable import NHSOnline

class KnownServicesTests: XCTestCase {
    let knownServices: KnownServices = KnownServices(config: config())
    
    func test_FindServiceForURL_ReturnsNilForInvalidUrl() {
        let service = self.knownServices.findMatchingKnownService(url: URL(string: "invalid url"))
        XCTAssertNil(service)
    }
    
    func test_FindServiceForUrl_ReturnsAService_WhenProvidedValidServiceUrl() {
        let service = self.knownServices.findMatchingKnownService(url: URL(string: "https://111.nhs.uk"))
        XCTAssertEqual("https://111.nhs.uk", service?.url.absoluteString)
    }
    
    func test_FindMatchingServiceInfo_ResolveToNil_WhenUrlIsUnknownServiceUrl() {
        let unknownServices = ["https://www.google.co.uk", "https://www.yahoo.co.uk", "https://www.jobs.nhs.uk"]
        unknownServices.forEach{unknownService in
            let service = self.knownServices.findMatchingKnownServiceInfo(url: URL(string: unknownService))
            XCTAssertNil(service)
        }
    }
    
    func test_FindMatchingServiceInfo_ResolveToNil_WhenServiceInternalPathLocationIsRelated() {
        let unrelatedServicePaths = [config().HomeUrl+"unrelatedPath", "https://111.nhs.uk/unrelatedPath"]
        unrelatedServicePaths.forEach {urlWithPath in
            let service = self.knownServices.findMatchingKnownServiceInfo(url: URL(string: urlWithPath))
            XCTAssertNil(service)
        }
    }
    
    func test_FindMatchingServiceInfo_ResolvesToCorrectNhsInternalServices() {
        var nhsoBaseUrl = config().HomeUrl
        if let lastChar = nhsoBaseUrl.last, lastChar == "/" {nhsoBaseUrl.removeLast() }
        let nhsoInternalServices = [nhsoBaseUrl, nhsoBaseUrl+config().SymptomsUrlPath,
                                    nhsoBaseUrl+config().AppointmentsUrlPath, nhsoBaseUrl+config().MyRecordUrlPath,
                                    nhsoBaseUrl+config().PrescriptionsUrlPath, nhsoBaseUrl+config().MoreUrlPath,
                                    nhsoBaseUrl+config().AppontmentsGpAtHandUrlPath, nhsoBaseUrl+config().MyRecordGpAtHandUrlPath,
                                    nhsoBaseUrl+config().PrescriptionsGpAtHandUrlPath, nhsoBaseUrl+config().InformaticaUrlPath]
        let equivalentHeaders = [getNSLocalString(key: "HomeTitle"), getNSLocalString(key: "SymptomsTitle"),
                                 getNSLocalString(key: "AppointmentsTitle"), getNSLocalString(key: "MyRecordTitle"),
                                 getNSLocalString(key: "PrescriptionsTitle"), getNSLocalString(key: "MoreTitle"),
                                 getNSLocalString(key: "ServiceUnavailableTitle"), getNSLocalString(key: "ServiceUnavailableTitle"),
                                 getNSLocalString(key: "ServiceUnavailableTitle"), getNSLocalString(key: "ServiceUnavailableTitle")]
        for index in 0..<nhsoInternalServices.count {
            guard let serviceInfo = self.knownServices.findMatchingKnownServiceInfo(url: URL(string: nhsoInternalServices[index])) else {
                assertionFailure("No service info found for \(nhsoInternalServices[index])")
                return
            }
            XCTAssertEqual(equivalentHeaders[index], serviceInfo.title)
        }
    }
    
    func test_FindMatchingServiceInfo_ResolvesToMatchingPathInfoOrClosestToThePath() {
        var nhsoBaseUrl = config().HomeUrl
        if let lastChar = nhsoBaseUrl.last, lastChar == "/" {nhsoBaseUrl.removeLast() }
        let nhsoInternalServices = ["\(nhsoBaseUrl+config().SymptomsUrlPath)/random-path",
                                    "\(nhsoBaseUrl+config().AppointmentsUrlPath)/random/path", nhsoBaseUrl+config().MyRecordUrlPath]
        let equivalentHeaders = [ getNSLocalString(key: "SymptomsTitle"),
                                 getNSLocalString(key: "AppointmentsTitle"), getNSLocalString(key: "MyRecordTitle")]
        for index in 0..<nhsoInternalServices.count {
            guard let serviceInfo = self.knownServices.findMatchingKnownServiceInfo(url: URL(string: nhsoInternalServices[index])) else {
                assertionFailure("No service info found for \(nhsoInternalServices[index])")
                return
            }
            XCTAssertEqual(equivalentHeaders[index], serviceInfo.title)
        }
    }
    
    func test_FindMatchingKnownServiceForHostname_ResolveToKnownServiceWhenHostIsValidValid_ButNilWhenNot() {
        let nhs111Host = URL(string: config().Nhs111Url)!.host
        let googleHost = URL(string: "https://www.google.co.uk/search")!.host
        let notValidHost = "not a host"
        
        guard let nhs111KnownService = knownServices.findMatchingKnownServiceForHostname(hostname: nhs111Host) else {
            assertionFailure("NHS 111 host should match to the NHS 111 known service object")
            return
        }
        XCTAssertEqual(nhs111Host, nhs111KnownService.url.host)
        let googleService = knownServices.findMatchingKnownServiceForHostname(hostname: googleHost)
        XCTAssertNil(googleService)
        let notValidService = knownServices.findMatchingKnownServiceForHostname(hostname: notValidHost)
        XCTAssertNil(notValidService)
    }
    
    func test_ShouldURLOpenExternally_ResolveToTrueWhenOneOfExternalServices_ButFalseWhenOneOfKnownServices() {
        let externalServiceUrl = URL(string: config().TermsAndConditionsURL)!
        let knownServiceUrl = URL(string: config().HomeUrl)!
        let externalServiceShouldOpenEx = knownServices.shouldURLOpenExternally(externalServiceUrl)
        let knownServiceShouldOpenEx = knownServices.shouldURLOpenExternally(knownServiceUrl)
        XCTAssertTrue(externalServiceShouldOpenEx, "External service should open externally")
        XCTAssertFalse(knownServiceShouldOpenEx, "Known service shouldn't open externally")
    }
    
    func test_IsSameHostAsHomeUrl_ReturnsTrueWhenUrlHostIsNhsoUrl_ButFalseWhenNot() {
        let homeUrl = URL(string: config().HomeUrl)
        let nhs111Url = URL(string: config().Nhs111Url)
        let homeUrlSameHostAsHome = knownServices.isSameHostAsHomeUrl(url: homeUrl)
        let nhs111UrlSameAsHomeHost = knownServices.isSameHostAsHomeUrl(url: nhs111Url)
        XCTAssertTrue(homeUrlSameHostAsHome)
        XCTAssertFalse(nhs111UrlSameAsHomeHost)
    }
    
    func test_theURLIsABiometricRedirectURL_ReturnsTrue() {
        let url = config().HomeUrl + config().BiometricRedirectURL
        print(url)
        let result = knownServices.isFidoAuthResponse(urlString: url)
        XCTAssertTrue(result, "Biometric URL should return true")
    }
    
    func test_theURLIsNotABiometricRedirectURL_ReturnsFalse() {
        let url = config().HomeUrl + "login?source=me"
        let result = knownServices.isFidoAuthResponse(urlString: url)
        XCTAssertFalse(result, "Biometric URL should return false")
    }
    
    private func getNSLocalString(key: String) -> String{
        return NSLocalizedString(key, comment: "")
    }
}
