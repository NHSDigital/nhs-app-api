import Foundation

class KnownServices {
    private let config:Config
    private var serviceList = Array<KnownService>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlString: config.HomeUrl, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.Nhs111Url, serviceTitle: config.TitleNHS111, shouldHandleUnavailability: true, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.OrganDonationUrl, serviceTitle: config.TitleOrganDonation, shouldHandleUnavailability: true, shouldAllowNativeInteraction:true, urlQueryString: config.NhsOnlineRequiredQueryString))
    }
    
    func findMatchingKnownServiceFor(url:URL) -> KnownService? {
        return findMatchingKnownServiceForHostname(hostname: url.host)
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
