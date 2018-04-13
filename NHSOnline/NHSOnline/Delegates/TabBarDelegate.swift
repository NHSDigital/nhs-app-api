import UIKit

class TabBarDelegate : NSObject, UITabBarDelegate {
    let viewController: HomeViewController
    
    enum Menu: Int {
        case Symptoms = 0
        case Appointments = 1
        case Prescription = 2
        case MyRecord = 3
        case More = 4
    }
    
    init(controller: HomeViewController) {
        viewController = controller
    }
    
    func tabBar(_ tabBar: UITabBar, didSelect: UITabBarItem) {
        let selectedItem = Menu(rawValue: didSelect.tag)!
        
        switch selectedItem {
        case .Symptoms:
            selectPage(pageUrl: config().Nhs111Url)
            break
        case .Appointments:
            let appointmentsUrl = appendPathToBaseUrl(urlPathToAppend: config().AppointmentsUrlPath)
            selectPage(pageUrl: appointmentsUrl)
            break
        case .Prescription:
            break
        case .MyRecord:
            break
        case .More:
            let moreUrl = appendPathToBaseUrl(urlPathToAppend: config().MoreUrlPath)
            selectPage(pageUrl: moreUrl)
            break
        }
    }
    
    private func selectPage(pageUrl: String) {
        viewController.pageUrl = pageUrl
        viewController.webViewController?.loadUrl(url: pageUrl)
    }
    
    private func appendPathToBaseUrl(urlPathToAppend: String) -> String {
        let url = NSURL(fileURLWithPath: config().HomeUrl).appendingPathComponent(urlPathToAppend)?.absoluteString
        return url!;
    }
}
