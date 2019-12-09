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
            viewController.selectedTab = menu.rawValue
        }
    }
    
    func tabBar(_ tabBar: UITabBar, didSelect: UITabBarItem) {
        if(!viewController.applicationState.isReady()) {
            selectMenu(menu: Menu(rawValue: viewController.selectedTab!)!)
            return
        }
        
        let selectedItem = Menu(rawValue: didSelect.tag)!
        
        if(viewController.selectedTab == didSelect.tag) {
            if(selectedItem != .More && selectedItem != .Symptoms) {
                return
            }
        }
        
        viewController.selectedTab = didSelect.tag

        var _: String
        if(selectedItem != .Symptoms) {
            viewController.applicationState.block()
        }
        processTabBarSelection(selectedTag: didSelect.tag)
    }
    
    
    func processTabBarSelection(selectedTag: Int) {
        let selectedItem = Menu(rawValue: selectedTag)!
        var selectedURL: String
        
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
        if (viewController.webViewController?.webView.url?.absoluteString != pageUrl) {
            viewController.webViewController?.webView.stopLoading()
            viewController.webViewController?.loadPage(url: pageUrl)
        }
    }
}
