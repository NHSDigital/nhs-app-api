import XCTest
import WebKit
@testable import NHSOnline

class TabBarDelegateTests : XCTestCase {
    var tabBarDelegate: TabBarDelegateMocks?
    var viewController: HomeViewController?
    var tabBarItem: UITabBarItem?
    var tabBar: UITabBar?
    
    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        viewController = (storyboard.instantiateViewController(withIdentifier: "HomeViewController") as! HomeViewController)
       
        tabBarDelegate = TabBarDelegateMocks(controller: viewController!)
        
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

    func test_WhenTheApplicationStateIsBusyTheMenuBarSelectionIsNotProcessedAndTheSelectedTabIsNotUpdated() {
        viewController!.applicationState.block()
        viewController!.selectedTab = 1
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled == false)
        XCTAssert(tabBarDelegate!.setMenuBarItemWasCalled == true)
        XCTAssert(tabBarDelegate!.setMenuBarItemWasCalledWith == 1)
        XCTAssert(viewController!.selectedTab == 1)
    }

    func test_WhenTheApplicationStateIsBusyAndSelectedTabIsNilThenTheSelectedTabRemainsNil() {
        viewController!.applicationState.block()
        viewController!.selectedTab = nil
        tabBarDelegate!.tabBar(tabBar!, didSelect: tabBarItem!)
        
        XCTAssert(tabBarDelegate!.processTabBarSelectionWasCalled == false)
        XCTAssert(tabBarDelegate!.setMenuBarItemWasCalled == true)
        XCTAssert(tabBarDelegate!.setMenuBarItemWasCalledWith == -1)
        XCTAssert(viewController!.selectedTab == nil)
    }
    
    func test_WhenSymptomsIsClickedOnThenTheApplicationStateIsNotBlocked() {
        let symptomsTabBarItem = UITabBarItem(title: "symptomsItem", image: nil, tag: 0)
        viewController!.selectedTab = symptomsTabBarItem.tag
        tabBarDelegate!.tabBar(tabBar!, didSelect: symptomsTabBarItem)
           
        XCTAssert(viewController!.applicationState.isReady())
    }
    
    func test_WhenMoreIsThePreviouslySelectedTabThenTheNextTabSelectionWillNotBlockTheApplicationState() {
        let moreTabBarItem = UITabBarItem(title: "moreItem", image: nil, tag: 4)
        viewController!.selectedTab = moreTabBarItem.tag
        tabBarDelegate!.tabBar(tabBar!, didSelect: moreTabBarItem)
             
        XCTAssert(viewController!.applicationState.isReady())
    }
    
    func test_WhenAppointmentsIsThePreviouslySelectedTabThenTheNextTabSelectionWillNotBlockTheApplicationState() {
        let appointmentsItemTabBar = UITabBarItem(title: "appointmentsItem", image: nil, tag: 1)
        viewController!.selectedTab = 1
        tabBarDelegate!.tabBar(tabBar!, didSelect: appointmentsItemTabBar)
             
        XCTAssert(viewController!.applicationState.isReady())
    }
    
    func test_WhenPrescriptionsIsThePreviouslySelectedTabThenTheNextTabSelectionWillNotBlockTheApplicationState() {
        let prescriptionsItemTabBar = UITabBarItem(title: "prescriptionsItem", image: nil, tag: 2)
        viewController!.selectedTab = 1
        tabBarDelegate!.tabBar(tabBar!, didSelect: prescriptionsItemTabBar)
             
        XCTAssert(viewController!.applicationState.isReady())
    }
}
