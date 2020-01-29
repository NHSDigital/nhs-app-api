import Foundation

class KnownServices {
    private let config:Config
    private let homeTitle = NSLocalizedString("HomeTitle", comment: "")
    private let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
    private let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
    private let accessibleNhsOnlineErrorMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
    private let nhs111Title = NSLocalizedString("NHS111Title", comment: "")
    private let accessibleNhs111Title = NSLocalizedString("AccessibleNHS111Title", comment: "")
    private let conditionsTitle = NSLocalizedString("ConditionsTitle", comment: "")
    private let accessibleConditionsTitle = NSLocalizedString("AccessibleConditionsTitle", comment: "")
    private let symptomsTitle = NSLocalizedString("SymptomsTitle", comment: "")
    private let appointmentsTitle = NSLocalizedString("AppointmentsTitle", comment: "")
    private let adminHelpTitle = NSLocalizedString("AdminHelpTitle", comment: "")
    private let prescriptionsTitle = NSLocalizedString("PrescriptionsTitle", comment: "")
    private let myRecordTitle = NSLocalizedString("MyRecordTitle", comment: "")
    private let moreTitle = NSLocalizedString("MoreTitle", comment: "")
    private let myAccountTitle = NSLocalizedString("MyAccountTitle", comment: "")
    private let organDonationTitle = NSLocalizedString("OrganDonationTitle", comment: "")
    private let dataPreferencesTitle = NSLocalizedString("DataPreferencesTitle", comment: "")
    private let serviceUnavailableErrorMessage = NSLocalizedString("ServiceUnavailableErrorMessage", comment: "")
    private let hotJarTitle = NSLocalizedString("HotJarTitle", comment: "")
    private let dataSharingTitle = NSLocalizedString("DataSharingTitle", comment: "")
    private let serviceUnavailableTitle = NSLocalizedString("ServiceUnavailableTitle", comment: "")
    private var serviceList = Array<KnownService>()
    private var externalSites = Array<URL>()
    
    init(config:Config) {
        self.config = config
       
        self.buildExternalSites()
        self.buildKnownServices()
    }
    
    func shouldURLOpenExternally(_ url: URL) -> Bool {
        return externalSites.contains(url)
    }
    
    func getUnavailabilityErrorMessageForService(_ url: URL?) -> ErrorMessage {
        guard findMatchingKnownServiceInfo(url: url) != nil else {
            return ErrorMessage(.ServiceUnavailable)
        }
        return ErrorMessage(.NoInternetConnection)
    }

    func findMatchingKnownServiceForHostname(hostname: String?) -> KnownService? {
        if let theHost = hostname {
            for service in serviceList {
                if service.url.host == theHost { return service }
            }
        }
        return nil
    }
    
    func isValidHomeUrl(url: URL?) -> Bool {
        if let homeUrl = URL(string: config.HomeUrl) {
            return homeUrl.host == url?.host && homeUrl.path == url?.path
        }
        return false
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
    
    func getPostRequestReloadUrl(url:URL) -> URL? {
        switch url {
        case  _ where url.absoluteString.starts(with: config.DataPreferencesURL):
            return URL(string: config.DataSharingUrlPath, relativeTo: URL(string: config.HomeUrl))
        default:
            return nil
        }
    }
    
    func isFidoAuthResponse(urlString: String) -> Bool {
        if let url = URL(string: urlString), let nhsUrl = URL(string: config.HomeUrl) {
            return url.host == nhsUrl.host &&
                urlString.contains(config.BiometricRedirectURL)
        }
        return false
    }
    
    private func buildExternalSites() {
        let helpURL: URL = URL(string: config.HelpURL)!
        let helpAppointmentsURL: URL = URL(string: config.HelpAppointmentsURL)!
        let HelpAccountURL: URL = URL(string: config.HelpAccountURL)!
        let HelpLoginURL: URL = URL(string: config.HelpLoginURL)!
        let helpPrescriptionsURL: URL = URL(string: config.HelpPrescriptionsURL)!
        let helpRecordURL: URL = URL(string: config.HelpRecordURL)!
        let termsAndConditionsURL: URL = URL(string: config.TermsAndConditionsURL)!
        let privacyPolicyURL: URL = URL(string: config.PrivacyPolicyURL)!
        let cookiesPolicyURL: URL = URL(string: config.CookiesPolicyURL)!
        let openSourceLicensesURL: URL = URL(string: config.OpenSourceLicencesURL)!
        let medicalRecordAbbreviationsURL: URL = URL(string: config.MedicalRecordAbbreviationsURL)!
        let accessibilityStatementURL: URL = URL(string: config.AccessibilityStatementURL)!
        
        let helpURLOld: URL = URL(string: config.HelpURLOld)!
        let termsAndConditionsURLOld: URL = URL(string: config.TermsAndConditionsURLOld)!
        let privacyPolicyURLOld: URL = URL(string: config.PrivacyPolicyURLOld)!
        let cookiesPolicyURLOld: URL = URL(string: config.CookiesPolicyURLOld)!
        let openSourceLicensesURLOld: URL = URL(string: config.OpenSourceLicencesURLOld)!
        let medicalRecordAbbreviationsURLOld: URL = URL(string: config.MedicalRecordAbbreviationsURLOld)!
        let accessibilityStatementURLOld: URL = URL(string: config.AccessibilityStatementURLOld)!
        
        let biometricsHelpURl: URL = URL(string: config.BiometricHelpURL)!
        let conditionsUrl: URL = URL(string: config.ConditionsUrlPath)!
        let dataSharingUrl: URL = URL(string: config.DataSharingUrlPath)!
        
        externalSites = [
            helpURL,
            HelpAccountURL,
            helpAppointmentsURL,
            HelpLoginURL,
            helpPrescriptionsURL,
            helpRecordURL,
            termsAndConditionsURL,
            privacyPolicyURL,
            cookiesPolicyURL,
            openSourceLicensesURL,
            medicalRecordAbbreviationsURL,
            accessibilityStatementURL,
            biometricsHelpURl,
            helpURLOld,
            termsAndConditionsURLOld,
            privacyPolicyURLOld,
            cookiesPolicyURLOld,
            openSourceLicensesURLOld,
            medicalRecordAbbreviationsURLOld,
            accessibilityStatementURLOld,
            conditionsUrl,
            dataSharingUrl
        ]
    }
    
    private func buildKnownServices() {
        let nhsoService = buildNhsoService()
        let nhs111LocationService = KnownService(serviceUrl: config.Nhs111LocationUrl, service: .NHS_111,  title: nhs111Title, validateSession: false, allowNativeInteraction: false)
        let dataPrefService = KnownService(serviceUrl: config.DataPreferencesURL, service: .DATA_PREFERENCES, title: dataPreferencesTitle, validateSession: false, allowNativeInteraction: true)
        
        self.serviceList.append(nhsoService)
        self.serviceList.append(nhs111LocationService)
        self.serviceList.append(dataPrefService)
    }

    private func buildNhsoService()-> KnownService {
        let nhsoService = KnownService(serviceUrl: config.HomeUrl, service: .NHS_ONLINE, title: homeTitle, validateSession: true, allowNativeInteraction: true)
        nhsoService.addPathInfo(path: config.SymptomsUrlPath, service: .SYMPTOMS, validateSession: true, allowNativeInteraction: true, title: symptomsTitle)
        nhsoService.addPathInfo(path: config.CheckSymptomsUrlPath, service: .NHS_ONLINE, validateSession: false,  allowNativeInteraction: false, title: symptomsTitle)
        nhsoService.addPathInfo(path: config.AppointmentsUrlPath, service: .APPOINTMENTS,validateSession: true,  allowNativeInteraction: true, title: appointmentsTitle)
        nhsoService.addPathInfo(path: config.AppontmentsGpAtHandUrlPath, service: .APPOINTMENTS,validateSession: true,  allowNativeInteraction: true, title: serviceUnavailableTitle)
        nhsoService.addPathInfo(path: config.InformaticaUrlPath, service: .APPOINTMENTS,validateSession: true,  allowNativeInteraction: true, title: serviceUnavailableTitle)
        nhsoService.addPathInfo(path: config.AdminHelpUrlPath, service: .ADMIN_HELP, validateSession: true,  allowNativeInteraction: true, title:adminHelpTitle)
        nhsoService.addPathInfo(path: config.PrescriptionsUrlPath, service: .PRESCRIPTIONS, validateSession: true, allowNativeInteraction: true, title: prescriptionsTitle)
        nhsoService.addPathInfo(path: config.PrescriptionsGpAtHandUrlPath, service: .PRESCRIPTIONS,validateSession: true,  allowNativeInteraction: true, title: serviceUnavailableTitle)
        nhsoService.addPathInfo(path: config.MyRecordUrlPath, service: .MY_RECORD,validateSession: true,  allowNativeInteraction: true, title: myRecordTitle)
        nhsoService.addPathInfo(path: config.MyRecordGpAtHandUrlPath, service: .MY_RECORD,validateSession: true,  allowNativeInteraction: true, title: serviceUnavailableTitle)
        nhsoService.addPathInfo(path: config.MoreUrlPath, service: .MORE, validateSession: true, allowNativeInteraction: true, title: moreTitle)
        nhsoService.addPathInfo(path: config.MyAccountUrlPath, service: .ACCOUNT, validateSession: true, allowNativeInteraction: true, title: myAccountTitle)
        nhsoService.addPathInfo(path: config.OrganDonationUrlPath, service: .ORGAN_DONATION, validateSession: true, allowNativeInteraction: true, title: organDonationTitle)
        return nhsoService
    }
    
    enum Service {
        case NHS_111, NHS_ONLINE, DATA_PREFERENCES, HOT_JAR, OTHERS, APPOINTMENTS, ADMIN_HELP, PRESCRIPTIONS, MY_RECORD, SYMPTOMS, MORE, ACCOUNT, ORGAN_DONATION;
    }
}


