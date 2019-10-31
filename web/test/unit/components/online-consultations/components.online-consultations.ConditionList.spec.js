import ConditionList from '@/components/online-consultations/ConditionList';
import MenuItem from '@/components/MenuItem';
import { mount, createStore } from '../../helpers';
import { redirectTo } from '@/lib/utils';
import each from 'jest-each';

jest.mock('@/lib/utils');

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      onlineConsultations: { demographicsConsentGiven: true },
      serviceJourneyRules: {
        rules: {
          cdssAdvice: { provider: 'stubs', serviceDefinition: 'NHS_ADVICE' },
        },
      },
    },
  }));

describe('condition list', () => {
  describe('template', () => {
    it('will have a parapgraph about selecting a condition', () => {
      // Arrange
      const conditionList = mount(ConditionList, {
        $store: $store(),
        propsData: { serviceDefinitions: [] },
      });

      // Act
      const paragraph = conditionList.find('#conditionInfo>p');

      // Assert
      expect(paragraph.text()).toEqual('translate_appointments.gp_advice.conditions.paragraph');
    });

    each([
      '#conditionInfo a',
      '#endConditionInfo a',
    ]).it('will have a button for general advice before and after the condition categories that when clicked evaluates general advice', (selector) => {
      // Arrange
      const onConditionClicked = jest.fn();
      const conditionList = mount(ConditionList, {
        $store: $store(),
        propsData: { serviceDefinitions: [{
          category: 'Allergies',
          items: [{ title: 'Hayfever', id: 'HFV' }],
        }] },
        methods: { onConditionClicked },
      });

      // Act
      const generalAdviceLink = conditionList.find(selector);
      generalAdviceLink.trigger('click.prevent');

      // Assert
      expect(generalAdviceLink.exists()).toBe(true);
      expect(generalAdviceLink.text()).toEqual('translate_appointments.gp_advice.conditions.link');
      expect(onConditionClicked).toHaveBeenCalledWith('NHS_ADVICE');
    });

    it('will have a menu item list for each category in service definitions', () => {
      // Arrange
      const conditionList = mount(ConditionList, {
        $store: $store(),
        propsData: { serviceDefinitions: [{
          category: 'Allergies',
          items: [{ title: 'Hayfever', id: 'HFV' }],
        }, {
          category: 'Breathing Problems',
          items: [
            { title: 'Bronchitis', id: 'BRC' },
            { title: 'COPD', id: 'CPD' },
          ],
        }] },
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
      const conditionList = mount(ConditionList, {
        $store: $store(),
        propsData: { serviceDefinitions: [] },
        methods: {
          endMyConsultationClicked,
        },
      });

      // Act
      const endMyConsultationButton = conditionList.find('#endMyConsultationButton');
      endMyConsultationButton.trigger('click.prevent');

      // Assert
      expect(endMyConsultationButton.element.innerHTML.trim()).toEqual('translate_onlineConsultations.orchestrator.endMyConsultationButton');
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
          propsData: { serviceDefinitions: [] },
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
        const conditionList = mount(ConditionList, {
          $store: $store(),
          propsData: { serviceDefinitions: [] },
        });

        // Act
        conditionList.vm.endMyConsultationClicked();

        // Assert
        expect(redirectTo).toHaveBeenCalledWith(conditionList.vm, '/');
      });
    });
  });
});
