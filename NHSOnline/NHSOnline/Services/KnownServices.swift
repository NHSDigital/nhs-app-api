import Foundation

class KnownServices {
    private let config:Config
    private var serviceList = Array<KnownService>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlStrings: [config.HomeUrl], serviceErrorMessage: config.ErrorMessageGeneric, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlStrings: [config.Nhs111Url, config.Nhs111LocationUrl], serviceTitle: config.TitleNHS111, serviceErrorMessage: config.ErrorMessageNhs111, shouldHandleUnavailability: true))
        serviceList.append(KnownService(urlStrings: [config.OrganDonationUrl], serviceTitle: config.TitleOrganDonation, serviceErrorMessage: config.ErrorMessageOrganDonation, shouldHandleUnavailability: true, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
    }
    
    func getAllKnownHosts() -> [String?] {
        let knownHosts = self.serviceList.flatMap { $0.urls }.map { $0.url?.host }
        return knownHosts
    }
    
    func getUnavailabilityErrorMessageForService(url:URL) -> String? {
        return ((findMatchingKnownServiceForHostname(hostname: url.host))?.serviceErrorMessage)!
    }
    
    func getGenericErrorMessage() -> String {
        return config.ErrorMessageGeneric
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
    
    func shouldAllowNativeInteraction(host:String?) -> Bool {
        if let knownService = findMatchingKnownServiceForHostname(hostname: host){
            return knownService.shouldAllowNativeInteraction
        }
        return false
    }
}
