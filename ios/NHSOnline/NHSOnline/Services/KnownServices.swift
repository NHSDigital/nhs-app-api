import Foundation

class KnownServices {
    private let config:Config
    private let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
    private let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
    private let nhs111Title = NSLocalizedString("NHS111Title", comment: "")
    private let organDonationTitle = NSLocalizedString("OrganDonationTitle", comment: "")
    private let conditionsTitle = NSLocalizedString("ConditionsTitle", comment: "")
    private let symptomsTitle = NSLocalizedString("SymptomsTitle", comment: "")
    private let dataSharingTitle = NSLocalizedString("DataSharingTitle", comment: "")
    private let serviceUnavailableErrorMessage = NSLocalizedString("ServiceUnavailableErrorMessage", comment: "")
    private let hotJarTitle = NSLocalizedString("HotJarTitle", comment: "")
    private var serviceList = Array<KnownService>()
    private var externalSites = Array<URL>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlStrings: [config.HomeUrl], service: .NHS_ONLINE, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.Nhs111Url, config.Nhs111LocationUrl], serviceTitle: nhs111Title, service: .NHS_111, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldValidateSession:false))
        serviceList.append(KnownService(urlStrings: [config.OrganDonationUrl], serviceTitle: organDonationTitle, service: .ORGAN_DONATION, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.DataSharingUrl], serviceTitle: dataSharingTitle, service: .DATA_SHARING, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.ConditionsUrlPath], serviceTitle: conditionsTitle, service: .CONDITIONS, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [getCheckSymptomsUrl()], serviceTitle: symptomsTitle, service: .NHS_ONLINE, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        
        
        let helpURL: URL = URL(string: config.HelpURL)!
        let termsAndConditionsURL: URL = URL(string: config.TermsAndConditionsURL)!
        let privacyPolicyURL: URL = URL(string: config.PrivacyPolicyURL)!
        let cookiesPolicyURL: URL = URL(string: config.CookiesPolicyURL)!
        let openSourceLicensesURL: URL = URL(string: config.OpenSourceLicensesURL)!
        let medicalRecordAbbreviationsURL: URL = URL(string: config.MedicalRecordAbbreviationsURL)!
        externalSites = [helpURL, termsAndConditionsURL, privacyPolicyURL, cookiesPolicyURL, openSourceLicensesURL, medicalRecordAbbreviationsURL]
    }
    
    func getCheckSymptomsUrl() -> String {
        let homeUrl = URL(string: config.HomeUrl)
        let url = URL(string: config.CheckSymptomsUrlPath, relativeTo: homeUrl)?.absoluteString
        return url!
    }
    
    func isHotJar(url: URL) -> Bool {
        let hotjarURL: URL = URL(string: config.HotJarLinkUrl)!
        if (url.host == hotjarURL.host) {
            return true
        }
        return false
    }
    
    func shouldURLOpenExternally(url: URL) -> Bool {
        if externalSites.contains(url) {
            return true
        }
        return false
    }
    
    func getAllKnownHosts() -> [String?] {
        let knownHosts = self.serviceList.flatMap { $0.urls }.map { $0.url?.host }
        return knownHosts
    }
    
    func getUnavailabilityErrorMessageForService(url:URL) -> ErrorMessage? {
        return ((findMatchingKnownServiceForHostname(hostname: url.host))?.serviceErrorMessage)!
    }
    
    func getServiceUnavailableErrorMessage() -> ErrorMessage {
        return ErrorMessage(title: serviceUnavailableErrorMessage)
    }
    
    func findMatchingKnownServiceForHostname(hostname: String?) -> KnownService? {
        if hostname != nil {
            for service in serviceList {
                for url in service.urls {
                    if (url.host == hostname) {
                        return service
                    }
                }
            }
        }
        return nil
    }
    
    func findMatchingKnownServiceForURL(url: URL?) -> KnownService? {
        if url != nil {
            for service in serviceList {
                for knownUrl in service.urls {
                    if (knownUrl.url?.host == url?.host && (knownUrl.url?.path == "" || knownUrl.url?.host == "/" || knownUrl.url?.path == url?.path)) {
                        return service
                    }
                }
            }
        }
        return nil
    }
    
    func shouldAllowNativeInteraction(host:String?) -> Bool {
        if let knownService = findMatchingKnownServiceForHostname(hostname: host){
            return knownService.shouldAllowNativeInteraction
        }
        return false
    }
    
    func shouldValidateSession(host:String?) -> Bool {
        if let knownService = findMatchingKnownServiceForHostname(hostname: host) {
            return knownService.shouldValidateSession
        }
        return true
    }
    
    enum Service {
        case NHS_111, CONDITIONS, NHS_ONLINE, ORGAN_DONATION, DATA_SHARING, OTHERS;
    }
}
