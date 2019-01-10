import Foundation

class KnownServices {
    private let config:Config
    private let homeTitle = NSLocalizedString("HomeTitle", comment: "")
    private let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
    private let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
    private let accessibleNhsOnlineErrorMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
    private let nhs111Title = NSLocalizedString("NHS111Title", comment: "")
    private let accessibleNhs111Title = NSLocalizedString("AccessibleNHS111Title", comment: "")
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
    private var genericError:ErrorMessage
    private var serviceList = Array<KnownService>()
    private var externalSites = Array<URL>()
    
    init(config:Config) {
        self.config = config
        self.genericError = ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage)
        self.buildExternalSites()
        self.buildKnownServices()
    }
    
    func shouldURLOpenExternally(_ url: URL) -> Bool {
        return externalSites.contains(url)
    }
    
    func getUnavailabilityErrorMessageForService(_ url: URL?) -> ErrorMessage {
        guard let serviceInfo = findMatchingKnownServiceInfo(url: url) else {
            return self.getServiceUnavailableErrorMessage()
        }
        return serviceInfo.serviceMessage
    }
    
    func getServiceUnavailableErrorMessage() -> ErrorMessage {
        return genericError
    }
    
    func getNoInternetConnectionErrorMessage() -> ErrorMessage {
        return genericError
    }
    
    func findMatchingKnownServiceForHostname(hostname: String?) -> KnownService? {
        if let theHost = hostname {
            for service in serviceList {
                if service.url.host == theHost { return service }
            }
        }
        return nil
    }
    
    func isSameHostAsHomeUrl(url: URL?) -> Bool {
        if let homeUrl = URL(string: config.HomeUrl) {
            return homeUrl.host == url?.host
        }
        return false
    }
    
    func shouldAllowNativeInteraction(host:String?) -> Bool {
        if let knownService = findMatchingKnownServiceForHostname(hostname: host){
            return knownService.defaultPathInfo.allowNativeInteraction
        }
        return false
    }
    
    func shouldValidateSession(host: String?) -> Bool {
        if let knownService = findMatchingKnownServiceForHostname(hostname: host) {
            return knownService.defaultPathInfo.validateSession
        }
        return true
    }
    
    func findMatchingKnownServiceInfo(url: URL?, withExactMatchingPath: Bool = false) -> KnownService.Info? {
        guard let theUrl = url, let matchingService = findMatchingKnownService(url: url) else { return nil }
        return matchingService.findMatchingServicePathInfo(urlString: theUrl.absoluteString)
    }
    
    func findMatchingKnownService(url: URL?) -> KnownService? {
        if let theUrl = url, let service = findMatchingKnownServiceForHostname(hostname: theUrl.host) {
            return service
        }
        return nil
    }
    func isCIDRedirectUrl(urlString:String) -> Bool {
        if let url = URL(string: urlString), let nhsUrl = URL(string: config.HomeUrl) {
            return url.host == nhsUrl.host && url.path == config.AuthRedirectPath
        }
        return false
    }
    
    private func buildExternalSites() {
        let helpURL: URL = URL(string: config.HelpURL)!
        let termsAndConditionsURL: URL = URL(string: config.TermsAndConditionsURL)!
        let privacyPolicyURL: URL = URL(string: config.PrivacyPolicyURL)!
        let cookiesPolicyURL: URL = URL(string: config.CookiesPolicyURL)!
        let openSourceLicensesURL: URL = URL(string: config.OpenSourceLicencesURL)!
        let medicalRecordAbbreviationsURL: URL = URL(string: config.MedicalRecordAbbreviationsURL)!
        let accessibilityStatementURL: URL = URL(string: config.AccessibilityStatementURL)!
        let biometricsHelpURl: URL = URL(string: config.BiometricHelpURL)!
        externalSites = [helpURL, termsAndConditionsURL, privacyPolicyURL, cookiesPolicyURL, openSourceLicensesURL, medicalRecordAbbreviationsURL, accessibilityStatementURL, biometricsHelpURl]
    }
    
    private func buildKnownServices() {
        let nhsoService = buildNhsoService()
        let conditionService = KnownService(serviceUrl: config.ConditionsUrlPath, service: .CONDITIONS, serviceError: genericError,  title: conditionsTitle, accessibleTitle: accessibleConditionsTitle, validateSession: false, allowNativeInteraction: true)
        let nhs111Service = KnownService(serviceUrl: config.Nhs111Url, service: .NHS_111, serviceError: genericError, title: nhs111Title, accessibleTitle: accessibleNhs111Title, validateSession: false, allowNativeInteraction: false)
        let nhs111LocationService = KnownService(serviceUrl: config.Nhs111LocationUrl, service: .NHS_111, serviceError: genericError, title: nhs111Title, validateSession: false, allowNativeInteraction: false)
        let organDonationService = buildDonationService()
        let dataPrefService = KnownService(serviceUrl: config.DataPreferencesURL, service: .DATA_PREFERENCES, serviceError: genericError, title: dataPreferencesTitle, validateSession: false, allowNativeInteraction: true)
        
        self.serviceList.append(nhsoService)
        self.serviceList.append(conditionService)
        self.serviceList.append(nhs111Service)
        self.serviceList.append(nhs111LocationService)
        self.serviceList.append(organDonationService)
        self.serviceList.append(dataPrefService)
    }
    
    private func buildDonationService()-> KnownService {
        let organDonationService = KnownService(serviceUrl: config.OrganDonationUrl, service: .ORGAN_DONATION, serviceError: genericError, title: organDonationTitle, validateSession: false, allowNativeInteraction: true)
        organDonationService.addPathInfo(path: config.NativeDonationPath, service: .ORGAN_DONATION, validateSession: false, allowNativeInteraction: true, title: organDonationTitle)
        return organDonationService
    }
    
    private func buildNhsoService()-> KnownService {
        let nhsoService = KnownService(serviceUrl: config.HomeUrl, service: .NHS_ONLINE, serviceError: genericError, title: homeTitle, validateSession: true, allowNativeInteraction: true, urlQueryString: config.NhsOnlineRequiredQueryString)
        nhsoService.addPathInfo(path: config.SymptomsUrlPath, service: .SYMPTOMS, validateSession: true, allowNativeInteraction: true, title: symptomsTitle)
        nhsoService.addPathInfo(path: config.CheckSymptomsUrlPath, service: .NHS_ONLINE, validateSession: false,  allowNativeInteraction: false, title: symptomsTitle)
        nhsoService.addPathInfo(path: config.AppointmentsUrlPath, service: .APPOINTMENTS,validateSession: true,  allowNativeInteraction: true, title: appointmentsTitle)
        nhsoService.addPathInfo(path: config.PrescriptionsUrlPath, service: .PRESCRIPTIONS, validateSession: true, allowNativeInteraction: true, title: prescriptionsTitle)
        nhsoService.addPathInfo(path: config.MyRecordUrlPath, service: .MY_RECORD,validateSession: true,  allowNativeInteraction: true, title: myRecordTitle)
        nhsoService.addPathInfo(path: config.MoreUrlPath, service: .MORE, validateSession: true, allowNativeInteraction: true, title: moreTitle)
        nhsoService.addPathInfo(path: config.MyAccountUrlPath, service: .ACCOUNT, validateSession: true, allowNativeInteraction: true, title: myAccountTitle)
        return nhsoService
    }
    
    enum Service {
        case NHS_111, CONDITIONS, NHS_ONLINE, ORGAN_DONATION, DATA_PREFERENCES, HOT_JAR, OTHERS, APPOINTMENTS, PRESCRIPTIONS, MY_RECORD, SYMPTOMS, MORE, ACCOUNT;
    }
}


