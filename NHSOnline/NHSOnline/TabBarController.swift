import UIKit

class TabBarController: UITabBarController, UITabBarControllerDelegate {
    
     @IBInspectable var defaultIndex: Int = 0
    
    override func viewDidLoad() {
        super.viewDidLoad()
        selectedIndex = defaultIndex
    }
}

