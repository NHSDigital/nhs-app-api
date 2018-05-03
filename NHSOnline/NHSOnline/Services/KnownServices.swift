import Foundation

class KnownServices {
    private let config:Config
    private var serviceList = Array<KnownService>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlString: config.HomeUrl, serviceErrorMessage: config.ErrorMessageGeneric, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.Nhs111Url, serviceTitle: config.TitleNHS111, serviceErrorMessage: config.ErrorMessageNhs111, shouldHandleUnavailability: true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.OrganDonationUrl, serviceTitle: config.TitleOrganDonation, serviceErrorMessage: config.ErrorMessageOrganDonation, shouldHandleUnavailability: true, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
    }
    
    func findMatchingKnownServiceFor(url:URL) -> KnownService? {
        return findMatchingKnownServiceForHostname(hostname: url.host)
    }
    
    func getUnavailabilityErrorMessageForService(url:URL) -> String? {
        return ((findMatchingKnownServiceForHostname(hostname: url.host))?.serviceErrorMessage)!
    }
    
    func getGenericErrorMessage() -> String {
        return config.ErrorMessageGeneric
    }
    
    func findMatchingKnownServiceForHostname(hostname: String?) -> KnownService? {
        if hostname != nil {
            if let index = serviceList.index(where: { $0.url.host == hostname }) {
                return serviceList[index]
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
