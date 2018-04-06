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
            viewController.webViewController?.loadUrl(url: config().Nhs111Url)
        case .Appointments:
            break
        case .Prescription:
            break
        case .MyRecord:
            break
        case .More:
            let moreUrl = NSURL(fileURLWithPath: config().BaseUrl).appendingPathComponent(config().MoreUrlPath)?.absoluteString
            viewController.webViewController?.loadUrl(url: moreUrl!)
            break
        }
    }
}
