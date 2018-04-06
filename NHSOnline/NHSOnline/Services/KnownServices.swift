import Foundation

class KnownServices {
    private let config:Config
    private var serviceList = Array<KnownService>()
    
    enum ServiceName {
        case NHS111, NHS_ONLINE, ORGAN_DONATION
    }
    
    init(config:Config) {
        self.config = config
        self.buildKnownServices()
    }
    
    func buildKnownServices() {
        serviceList.append(KnownService(urlString: config.BaseUrl, urlQueryString: config.NhsOnlineRequiredQueryString))
        serviceList.append(KnownService(urlString: config.Nhs111Url, shouldHandleUnavailability: true))
        serviceList.append(KnownService(urlString: config.OrganDonationUrl))
    }
    
    func findMatchingKnownServiceFor(url:URL)-> KnownService? {
        if let i = serviceList.index(where: { $0.url.host == url.host }) {
            return serviceList[i]
        }
        return nil
    }
    
    func isTheService(knownService:KnownService, name:ServiceName)-> Bool {
        switch name {
        case .NHS_ONLINE:
            return knownService.url.host == URLComponents(string: config.BaseUrl)?.host
        case .NHS111:
            return knownService.url.host == URLComponents(string: config.Nhs111Url)?.host
        case .ORGAN_DONATION:
            return knownService.url.host == URLComponents(string: config.OrganDonationUrl)?.host
        }
    }
}
