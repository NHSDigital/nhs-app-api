@testable import NHSOnline

class TabBarDelegateMocks : TabBarDelegate {
    
    var processTabBarSelectionWasCalled = false
    var setMenuBarItemWasCalled = false
    var setMenuBarItemWasCalledWith: Int? = nil
    
    override init(controller: HomeViewController) {
        super.init(controller: controller)
    }
    
    override func processTabBarSelection(selectedTag: Int) {
        processTabBarSelectionWasCalled = true
    }
    
    override func setMenuBarItem(index: Int) {
        super.setMenuBarItem(index: index)
        setMenuBarItemWasCalled = true
        setMenuBarItemWasCalledWith = index
    }
}
