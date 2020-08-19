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

    func setMenuBarItem(index: Int){
        if let tabBar = viewController.tabBar, let items = tabBar.items {
            if(index >= 0 && index < items.count){
                tabBar.selectedItem = items[index]
                viewController.selectedTab = index
            }
        }
    }

    func setMenuBarItem(menuTab: MenuTab){
        var index: Int
        switch menuTab {
            case .Symptoms:
                index = 0
            case .Appointments:
                index = 1
            case .Prescriptions:
                index = 2
            case .MyRecord:
                index = 3
            case .More:
                index = 4
            default:
                index = -1
        }
        setMenuBarItem(index: index)
    }
    
    func tabBar(_ tabBar: UITabBar, didSelect: UITabBarItem) {
        if(!viewController.applicationState.isReady()) {
            setMenuBarItem(index: viewController.selectedTab!)
            return
        }
        
        let previouslySelectedItem = Menu(rawValue: viewController.selectedTab ?? 0)
        let currentlySelectedItem = Menu(rawValue: didSelect.tag)!
        
        if(viewController.selectedTab == didSelect.tag) {
            if(currentlySelectedItem != .More && currentlySelectedItem != .Symptoms && currentlySelectedItem != .Appointments && currentlySelectedItem != .Prescriptions) {
                return
            }
        }
        
        viewController.selectedTab = didSelect.tag

        var _: String
        if(currentlySelectedItem != .Symptoms && previouslySelectedItem != .More && currentlySelectedItem != .Appointments && currentlySelectedItem != .Prescriptions) {
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
            selectedURL = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().HealthRecordsUrlPath)
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
