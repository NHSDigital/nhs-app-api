import Foundation

class KnownServices {
    private let config:Config
    private var serviceList = Array<KnownService>()
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlString: config.HomeUrl, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.Nhs111Url, shouldHandleUnavailability: true))
        serviceList.append(KnownService(urlString: config.OrganDonationUrl))
    }
    
    func findMatchingKnownServiceFor(url:URL) -> KnownService? {
        if let i = serviceList.index(where: { $0.url.host == url.host }) {
            return serviceList[i]
        }
        
        return nil
    }
}
