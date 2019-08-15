import { mount, createStore, createRouter } from '../../../helpers';
import Conditions from '@/pages/appointments/gp-advice/conditions';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

describe('Conditions page', () => {
  let page;
  let $store;
  let $router;

  const createWrapper = () => {
    $router = createRouter();
    $store = createStore({
      dispatch: jest.fn(),
      state: {
        device: {
          isNativeApp: true,
        },
        onlineConsultations: {
          error: false,
          serviceDefinitions: [{
            category: 'Test Category',
            items: [
              { id: 'testId1', title: 'condition1' },
              { id: 'testId2', title: 'condition2' },
              { id: 'testId3', title: 'condition3' },
            ],
          }],
        },
        serviceJourneyRules: {
          rules: {
            cdssAdvice: {
              provider: 'stubs',
            },
          },
        },
      },
    });
    return mount(Conditions, { $store, $router });
  };

  describe('Computed properties', () => {
    it('will get the service definitions', () => {
      page = createWrapper();
      const serviceDefs = $store.state.onlineConsultations.serviceDefinitions;
      expect(page.vm.serviceDefinitions).toBe(serviceDefs);
    });

    it('will get the error from the store', () => {
      page = createWrapper();
      expect(page.vm.isError).toBe(false);
    });

    it('will get isNativeApp from the store', () => {
      page = createWrapper();
      expect(page.vm.isNativeApp).toBe(true);
    });
  });

  describe('template', () => {
    it('will have information about clicking a condition', () => {
      page = createWrapper();
      expect(page.find('#conditionInfo p').text()).toEqual('translate_appointments.gp_advice.conditions.paragraph');
      expect(page.find('#conditionInfo a').text()).toEqual('translate_appointments.gp_advice.conditions.link');
    });

    it('will contain the correct conditions', () => {
      page = createWrapper();
      const tagArray = page.findAll(AnalyticsTrackedTag);
      expect(tagArray.length).toBe(3);
      expect(page.find('div h2').text()).toEqual('Test Category');
      expect(tagArray.at(0).text()).toEqual('condition1');
      expect(tagArray.at(1).text()).toEqual('condition2');
      expect(tagArray.at(2).text()).toEqual('condition3');
    });
  });

  describe('methods', () => {
    describe('asyncData', () => {
      it('will dispatch onlineConsultations getServiceDefinitions action passing cdssAdvice provider', () => {
        // Arrange
        const expectedParams = { provider: 'stubs' };
        page = createWrapper();

        // Act
        page.vm.$options.asyncData({ store: $store });

        // Assert
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/getServiceDefinitions', expectedParams);
      });
    });
    describe('getConditionHref', () => {
      it('return GP Advice path with serviceDefinitionId query parameter equal to its parameter', () => {
        // Arrange
        page = createWrapper();
        const expectedResult = '/appointments/gp-advice?serviceDefinitionId=NHS_ADMIN';

        // Act
        const result = page.vm.getConditionHref('NHS_ADMIN');

        // Assert
        expect(result).toEqual(expectedResult);
      });
    });
  });
});
