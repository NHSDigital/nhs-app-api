import UIKit

extension UINavigationController {
    open override var preferredStatusBarStyle: UIStatusBarStyle {
        return .lightContent
    }
    override open var childForStatusBarStyle: UIViewController? {
      return topViewController
    }

    override open var childForStatusBarHidden: UIViewController? {
      return topViewController
    }
}
