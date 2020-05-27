@testable import NHSOnline

class TabBarDelegateMocks : TabBarDelegate {
    
    var processTabBarSelectionWasCalled = false
    
    override init(controller: HomeViewController) {
        super.init(controller: controller)
    }
    
    override func processTabBarSelection(selectedTag: Int) {
        processTabBarSelectionWasCalled = true
    }
}
