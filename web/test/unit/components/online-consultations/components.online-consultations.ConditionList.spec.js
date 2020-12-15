import i18n from '@/plugins/i18n';
import each from 'jest-each';
import ConditionList from '@/components/online-consultations/ConditionList';
import MenuItem from '@/components/MenuItem';
import { INDEX_PATH } from '@/router/paths';
import { redirectTo,
  CHILD_DEFAULT_SERVICE_DEFINITION,
  ADULT_DEFAULT_SERVICE_DEFINITION,
} from '@/lib/utils';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils');

const defaultServiceDefinitions = [{
  category: 'Allergies',
  items: [{ title: 'Hayfever', id: 'HFV' }],
},
{
  category: 'Allergies1',
  items: [{ title: 'Hayfever1', id: 'HFV1' }],
}];

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      onlineConsultations: { demographicsConsentGiven: true },
      serviceJourneyRules: {
        rules: {
          cdssAdvice: { provider: 'stubs' },
        },
      },
    },
  }));

const mountComponent = ({
  store,
  serviceDefinitions = defaultServiceDefinitions,
  isChildJourney,
  methods = {},
} = {}) =>
  mount(ConditionList, {
    $store: store || $store(),
    propsData: {
      serviceDefinitions,
    },
    methods,
    computed: {
      isChildJourney() {
        return isChildJourney;
      },
    },
    mountOpts: {
      i18n,
    },
  });

describe('condition list', () => {
  describe('template', () => {
    it('will have a parapgraph about selecting a condition', () => {
      const conditionList = mountComponent();

      // Act
      const paragraph = conditionList.find('#conditionInfo>p');

      // Assert
      expect(paragraph.text()).toEqual('To ensure we ask you relevant questions, choose your condition.');
    });

    each([
      '#conditionInfo a',
      '#endConditionInfo a',
    ]).it('will have a button for general advice before and after the condition categories that when clicked evaluates general advice', (selector) => {
      // Arrange
      const onConditionClicked = jest.fn();
      const store = createStore({
        state: {
          device: { isNativeApp: true },
          serviceJourneyRules: {
            rules: {
              cdssAdvice: { provider: 'stubs' },
            },
          },
          onlineConsultations: {
            defaultCondition: 'DEFAULT_CONDITION',
            demographicsConsentGiven: true,
          },
        },
      });

      const conditionList = mountComponent({
        store,
        methods: {
          onConditionClicked,
        },
      });

      // Act
      const generalAdviceLink = conditionList.find(selector);
      generalAdviceLink.trigger('click.prevent');

      // Assert
      expect(generalAdviceLink.exists()).toBe(true);
      expect(generalAdviceLink.text()).toEqual('I cannot find my condition');
      expect(onConditionClicked).toHaveBeenCalledWith('DEFAULT_CONDITION');
    });

    it('will have a menu item list for each category in service definitions', () => {
      // Arrange
      const conditionList = mountComponent({
        serviceDefinitions: [{
          category: 'Allergies',
          items: [{ title: 'Hayfever', id: 'HFV' }],
        },
        {
          category: 'Breathing Problems',
          items: [
            { title: 'Bronchitis', id: 'BRC' },
            { title: 'COPD', id: 'CPD' },
          ],
        }],
      });

      // Act
      const categories = conditionList.findAll('.conditionCategory').wrappers;
      const headings = categories.map(c => c.find('h2').text());
      const hayFeverCondition = categories[0].find(MenuItem);
      const breathingProblemsConditions = categories[1].findAll('button').wrappers.map(b => b.text());

      // Assert
      expect(headings).toEqual(['Allergies', 'Breathing Problems']);
      expect(hayFeverCondition.vm.text).toEqual('Hayfever');
      expect(hayFeverCondition.vm.clickParam).toEqual('HFV');
      expect(breathingProblemsConditions).toEqual(['Bronchitis', 'COPD']);
    });

    it('will have an end my consultation button', () => {
      // Arrange
      const endMyConsultationClicked = jest.fn();
      const conditionList = mountComponent({
        methods: {
          endMyConsultationClicked,
        },
      });

      // Act
      const endMyConsultationButton = conditionList.find('#endMyConsultationButton');
      endMyConsultationButton.trigger('click.prevent');

      // Assert
      expect(endMyConsultationButton.element.innerHTML.trim()).toEqual('End my consultation');
      expect(endMyConsultationClicked).toHaveBeenCalled();
    });
  });

  describe('methods', () => {
    describe('onConditionClicked', () => {
      it('will dispatch evaluate and set selected service definition', async () => {
        // Arrange
        const store = $store();
        const conditionList = mount(ConditionList, {
          $store: store,
          propsData: {
            serviceDefinitions: [{
              category: 'Allergies',
              items: [{ title: 'Hayfever', id: 'HFV' }],
            },
            {
              category: 'Allergies1',
              items: [{ title: 'Hayfever1', id: 'HFV1' }],
            }] },
          mountOpts: {
            i18n,
          },
        });

        // Act
        await conditionList.vm.onConditionClicked('HFV');

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', {
          serviceDefinitionId: 'HFV',
          provider: 'stubs',
          answeringConditionsQuestion: true,
        });
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setGpAdviceServiceDefinitionId', 'HFV');
      });
    });

    describe('endMyConsultationClicked', () => {
      it('will dispatch evaluate and set selected service definition', () => {
        // Arrange
        const conditionList = mountComponent();

        // Act
        conditionList.vm.endMyConsultationClicked();

        // Assert
        expect(redirectTo).toHaveBeenCalledWith(conditionList.vm, INDEX_PATH);
      });
    });
  });

  describe('computed', () => {
    each([
      ['DEFAULT_CONDITION', 2],
      [undefined, 0],
    ]).it('will not show the cannot find my condition links if there is no default condition',
      (defaultCondition, expectedLinksVisible) => {
        const store = createStore({
          state: {
            device: { isNativeApp: true },
            serviceJourneyRules: {
              rules: {
                cdssAdvice: { provider: 'stubs' },
              },
            },
            onlineConsultations: {
              defaultCondition,
              demographicsConsentGiven: true,
            },
          },
        });
        const conditionList = mountComponent({ store });
        const cannotFindLinks = conditionList.findAll('#cannotFindConditionLink');

        expect(cannotFindLinks.length).toBe(expectedLinksVisible);
      });

    each([
      [true, 'To ensure we ask you relevant questions, choose your child\'s condition.'],
      [false, 'To ensure we ask you relevant questions, choose your condition.'],
    ]).it('will not show the correct title',
      (isChildJourney, expectedTitle) => {
        const conditionList = mountComponent({ isChildJourney });

        expect(conditionList.find('#conditionListTitle').text()).toEqual(expectedTitle);
      });

    each([
      [ADULT_DEFAULT_SERVICE_DEFINITION, 'I cannot find my condition'],
      [CHILD_DEFAULT_SERVICE_DEFINITION, 'I cannot find my child\'s condition'],
    ])
      .it('will show the correct link text based on the default condition',
        (defaultCondition, expectedLinkText) => {
          const store = createStore({
            state: {
              device: { isNativeApp: true },
              serviceJourneyRules: {
                rules: {
                  cdssAdvice: { provider: 'stubs' },
                },
              },
              onlineConsultations: {
                defaultCondition,
                demographicsConsentGiven: true,
              },
            },
          });

          const conditionList = mountComponent({ store });
          const cannotFindLinks = conditionList.findAll('#cannotFindConditionLink');

          expect(cannotFindLinks.at(0).text()).toEqual(expectedLinkText);
          expect(cannotFindLinks.at(1).text()).toEqual(expectedLinkText);
        });
  });

  describe('created', () => {
    it('will automatically select condition if there only is one', () => {
      // Arrange
      const store = $store();
      mountComponent({
        store,
        serviceDefinitions: [
          {
            category: 'Allergies',
            items: [{ title: 'Hayfever', id: 'HFV' }],
          }],
      });

      // Assert
      expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', {
        serviceDefinitionId: 'HFV',
        provider: 'stubs',
        answeringConditionsQuestion: true,
      });
      expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setServiceDefinitionId', 'HFV');
    });
  });
});
