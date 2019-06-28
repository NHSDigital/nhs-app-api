import XCTest
import WebKit
@testable import NHSOnline

class TabBarDelegateTests : XCTestCase {
    var tabBarDelegate: MockTabBarDelegate?
    var viewController: HomeViewController?
    var tabBarItem: UITabBarItem?
    var tabBar: UITabBar?
    
    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        viewController = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as! HomeViewController
       
        tabBarDelegate = MockTabBarDelegate(controller: viewController!)
        
        tabBarItem = UITabBarItem(title: "testItem", image: nil, tag: 2)
        let rect = CGRect(x: 10, y: 10, width: 10, height: 10)
        tabBar = UITabBar(frame: rect)
    }
    
    func test_WhenTheMenuBarIsClickedForTheFirstTimeTheCurrentMenuItemIsStored() {
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarItem!.tag == viewController!.selectedTab)
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled)
    }
    
    func test_WhenANewItemIsClickedOnTheMenuBarTheStoredIdIsUpdatedAndTheSelectionIsProcessed() {
        viewController!.selectedTab = 1
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarItem!.tag == viewController!.selectedTab)
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled)
    }
    
    func test_WhenASelectedItemIsClickedOnTheMenuBarTheSelectionIsNotProcessed() {
        viewController!.selectedTab = 2
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled == false)
    }
    
    func test_WhenTheApplicationStateIsBusyTheMenuBarSelectionIsNotProcessedAndTheSelectedTabIsNotUpdated() {
        viewController!.applicationState.block()
        viewController!.selectedTab = 1
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled == false)
        XCTAssert(viewController!.selectedTab==1)
    }
}

class MockTabBarDelegate : TabBarDelegate {
    
    var processTabBarSelectionWasCalled = false
    override init(controller: HomeViewController) {
        super.init(controller: controller)
    }
    
    
    override func processTabBarSelection(selectedTag: Int) {
        processTabBarSelectionWasCalled = true
    }
}
