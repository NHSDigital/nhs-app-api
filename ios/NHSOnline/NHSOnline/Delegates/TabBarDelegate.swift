import UIKit

class TabBarDelegate : NSObject, UITabBarDelegate {
    let viewController: HomeViewController
    
    enum Menu: Int {
        case Symptoms = 0
        case Appointments = 1
        case Prescriptions = 2
        case MyRecord = 3
        case More = 4
    }
    
    init(controller: HomeViewController) {
        viewController = controller
    }
    
    func selectMenu(menu: Menu) {
        if let tabBar = viewController.tabBar, let items = tabBar.items {
            tabBar.selectedItem = items[menu.rawValue] as UITabBarItem
        }
    }
    
    func tabBar(_ tabBar: UITabBar, didSelect: UITabBarItem) {
        if(viewController.selectedTab == didSelect.tag) {
            return
        }
        
        if(!viewController.applicationState.isReady()) {
            selectMenu(menu: Menu(rawValue: viewController.selectedTab!)!)
            return
        }
        viewController.selectedTab = didSelect.tag
        
        let selectedItem = Menu(rawValue: didSelect.tag)!
        var selectedURL: String

        viewController.applicationState.block()
        switch selectedItem {
            case .Symptoms:
                selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().SymptomsUrlPath)
                break
            case .Appointments:
                selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().AppointmentsUrlPath)
                break
            case .Prescriptions:
                selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().PrescriptionsUrlPath)
                break
            case .MyRecord:
                selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().MyRecordUrlPath)
                break
            case .More:
                selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().MoreUrlPath)
                break
        }
        selectPage(pageUrl: selectedURL)
    }
    
    private func selectPage(pageUrl: String) {
        if (viewController.webViewController?.webView.url?.absoluteString != pageUrl + config().NhsOnlineRequiredQueryString) {
            viewController.webViewController?.webView.stopLoading()
            viewController.webViewController?.loadPage(url: pageUrl)
        }
    }
}
