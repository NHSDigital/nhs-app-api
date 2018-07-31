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
        let selectedItem = Menu(rawValue: didSelect.tag)!
        switch selectedItem {
        case .Symptoms:
            let symptomsUrl = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().SymptomsUrlPath)
            selectPage(pageUrl: symptomsUrl)
            self.viewController.updateHeaderText(headerText: NSLocalizedString("SymptomsTitle", comment: ""))
            break
        case .Appointments:
            let appointmentsUrl = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().AppointmentsUrlPath)
            selectPage(pageUrl: appointmentsUrl)
            self.viewController.updateHeaderText(headerText: NSLocalizedString("AppointmentsTitle", comment: ""))
            break
        case .Prescriptions:
            let prescriptionsUrl = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().PrescriptionsUrlPath)
            selectPage(pageUrl: prescriptionsUrl)
            self.viewController.updateHeaderText(headerText: NSLocalizedString("PrescriptionsTitle", comment: ""))

            break
        case .MyRecord:
            let myRecordUrl = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().MyRecordUrlPath)
            selectPage(pageUrl: myRecordUrl)
            self.viewController.updateHeaderText(headerText: NSLocalizedString("MyRecordTitle", comment: ""))
            break
        case .More:
            let moreUrl = viewController.createHomeUrlSubRequestWithPath(urlPathToAppend: config().MoreUrlPath)
            selectPage(pageUrl: moreUrl)
            self.viewController.updateHeaderText(headerText: NSLocalizedString("MoreTitle", comment: ""))
            break
        }
    }
    
    private func selectPage(pageUrl: String) {
        viewController.pageUrl = pageUrl
        viewController.webViewController?.webView.loadPage(url: pageUrl)
    }
}
