import UIKit

extension UITabBar {
    func setDefaultTabBarItemsAppearance() {
        items?.forEach({ (uiTabBarItem) in
            uiTabBarItem.setTitleTextAttributes([NSAttributedStringKey.foregroundColor: UIColor(red:0.26, green:0.33, blue:0.39, alpha:1.0)], for: UIControlState.normal)
            uiTabBarItem.setTitleTextAttributes([NSAttributedStringKey.foregroundColor:UIColor(red:0.00, green:0.37, blue:0.72, alpha:1.0)], for: UIControlState.highlighted)
        })
    }
}
