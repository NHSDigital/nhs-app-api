import Foundation

class KnownServices {
    private let config:Config
    private let homeTitle = NSLocalizedString("HomeTitle", comment: "")
    private let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
    private let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
    private let accessibleNhsOnlineErrorMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
    private let nhs111Title = NSLocalizedString("NHS111Title", comment: "")
    private let organDonationTitle = NSLocalizedString("OrganDonationTitle", comment: "")
    private let conditionsTitle = NSLocalizedString("ConditionsTitle", comment: "")
    private let accessibleConditionsTitle = NSLocalizedString("AccessibleConditionsTitle", comment: "")
    private let symptomsTitle = NSLocalizedString("SymptomsTitle", comment: "")
    private let appointmentsTitle = NSLocalizedString("AppointmentsTitle", comment: "")
    private let prescriptionsTitle = NSLocalizedString("PrescriptionsTitle", comment: "")
    private let myRecordTitle = NSLocalizedString("MyRecordTitle", comment: "")
    private let moreTitle = NSLocalizedString("MoreTitle", comment: "")
    private let myAccountTitle = NSLocalizedString("MyAccountTitle", comment: "")
    private let dataPreferencesTitle = NSLocalizedString("DataPreferencesTitle", comment: "")
    private let serviceUnavailableErrorMessage = NSLocalizedString("ServiceUnavailableErrorMessage", comment: "")
    private let hotJarTitle = NSLocalizedString("HotJarTitle", comment: "")
    private var serviceList = Array<KnownService>()
    private var externalSites = Array<URL>()
    private var internalSerivceList = Array<KnownService>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
        self.buildInternalServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlStrings: [config.HomeUrl], serviceTitle: homeTitle, service: .NHS_ONLINE, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.Nhs111Url, config.Nhs111LocationUrl], serviceTitle: nhs111Title, service: .NHS_111, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldValidateSession:false))
        serviceList.append(KnownService(urlStrings: [config.OrganDonationUrl], serviceTitle: organDonationTitle, service: .ORGAN_DONATION, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.DataPreferencesURL], serviceTitle: dataPreferencesTitle, service: .DATA_PREFERENCES, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false))
        serviceList.append(KnownService(urlStrings: [config.ConditionsUrlPath], serviceTitle: conditionsTitle, accessibleServiceTitle: accessibleConditionsTitle, service: .CONDITIONS, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: accessibleNhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [getCheckSymptomsUrl()], serviceTitle: symptomsTitle, service: .NHS_ONLINE, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage), shouldAllowNativeInteraction:true, shouldValidateSession:false,urlQueryString: config.NhsOnlineRequiredQueryString))
        
        
        let helpURL: URL = URL(string: config.HelpURL)!
        let termsAndConditionsURL: URL = URL(string: config.TermsAndConditionsURL)!
        let privacyPolicyURL: URL = URL(string: config.PrivacyPolicyURL)!
        let cookiesPolicyURL: URL = URL(string: config.CookiesPolicyURL)!
        let openSourceLicensesURL: URL = URL(string: config.OpenSourceLicensesURL)!
        let medicalRecordAbbreviationsURL: URL = URL(string: config.MedicalRecordAbbreviationsURL)!
        externalSites = [helpURL, termsAndConditionsURL, privacyPolicyURL, cookiesPolicyURL, openSourceLicensesURL, medicalRecordAbbreviationsURL]
    }
    
    func buildInternalServices() {
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.SymptomsUrlPath)], serviceTitle: symptomsTitle, service: .SYMPTOMS, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.AppointmentsUrlPath)], serviceTitle: appointmentsTitle, service: .APPOINTMENTS, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.PrescriptionsUrlPath)], serviceTitle: prescriptionsTitle, service: .PRESCRIPTIONS, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.MyRecordUrlPath)], serviceTitle: myRecordTitle, service: .MY_RECORD, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.MoreUrlPath)], serviceTitle: moreTitle, service: .MORE, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))
        internalSerivceList.append(KnownService(urlStrings: [getFullInternalUrl(urlPath: config.MyAccountUrlPath)], serviceTitle: myAccountTitle, service: .ACCOUNT, serviceErrorMessage: ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage)))        
    }
    
    func getCheckSymptomsUrl() -> String {
        let homeUrl = URL(string: config.HomeUrl)
        let url = URL(string: config.CheckSymptomsUrlPath, relativeTo: homeUrl)?.absoluteString
        return url!
    }
    
    private func getFullInternalUrl(urlPath: String) -> String {
        let homeUrl = URL(string: config.HomeUrl)
        let url = URL(string: urlPath, relativeTo: homeUrl)?.absoluteString
        return url!
    }
    
    func getAllKnownHosts() -> [String?] {
        let knownHosts = self.serviceList.flatMap { $0.urls }.map { $0.url?.host }
        return knownHosts
    }
    
    func shouldURLOpenExternally(url: URL) -> Bool {
        if externalSites.contains(url) {
            return true
        }
        return false
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
        return findServiceforURL(url: url, services: serviceList)
    }
    
    func findMatchingInternalServiceForURL(url: URL?) -> KnownService? {
        return findServiceforURL(url: url, services: internalSerivceList)
    }
    
    func isSameHostAsHomeUrl(url: URL?) -> Bool {
        if let homeUrl = URL(string: config.HomeUrl) {
            return homeUrl.host == url?.host
        }
        return false
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
    
    private func findServiceforURL(url: URL?, services: [KnownService]) -> KnownService? {
        if url != nil {
            for service in services {
                for knownUrl in service.urls {
                    if (knownUrl.url?.host == url?.host && (knownUrl.url?.path == "" || knownUrl.url?.host == "/" || knownUrl.url?.path == url?.path)) {
                        return service
                    }
                }
            }
        }
        return nil
    }
    
    enum Service {
        case NHS_111, CONDITIONS, NHS_ONLINE, ORGAN_DONATION, DATA_PREFERENCES, HOT_JAR, OTHERS, APPOINTMENTS, PRESCRIPTIONS, MY_RECORD, SYMPTOMS, MORE, ACCOUNT;
    }
}
