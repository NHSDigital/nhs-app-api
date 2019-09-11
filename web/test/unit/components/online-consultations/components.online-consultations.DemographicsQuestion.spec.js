import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import GenericButton from '@/components/widgets/GenericButton';
import { mount, shallowMount, createStore, createScrollTo } from '../../helpers';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import each from 'jest-each';

jest.mock('@/services/event-bus');
jest.mock('@/services/native-app');

const defaultStore = () => (
  createStore({
    state: {
      onlineConsultations: {},
      device: { isNativeApp: false },
    },
  })
);

const mountComponent = ({
  shallow = true,
  $store = defaultStore(),
  slots = undefined,
  methods = undefined,
} = {}) => (
  (shallow ? shallowMount : mount)(DemographicsQuestion, {
    $store,
    propsData: {
      provider: 'stubs',
      serviceDefinitionId: 'NHS_ADMIN',
      checkboxLabel: 'I consent',
    },
    slots,
    methods,
  })
);

describe('demographics question', () => {
  describe('props', () => {
    each([
      'provider',
      'serviceDefinitionId',
      'checkboxLabel',
    ]).it('is required and is a String', (propName) => {
      const component = mountComponent();
      const prop = component.vm.$options.props[propName];

      expect(prop.required).toBeTruthy();
      expect(prop.type).toBe(String);
    });
  });
  describe('computed props', () => {
    describe('demographicsAnswer', () => {
      it('will read from the olc store answer', () => {
        // Arrange
        const $store = defaultStore();
        $store.state.onlineConsultations.answer = ['CHOICE_1'];

        // Act
        const component = mountComponent({
          $store,
        });

        // Assert
        expect(component.vm.demographicsAnswer).toEqual(['CHOICE_1']);
      });
      it('will update olc store answer when set', () => {
        // Arrange
        const $store = defaultStore();
        $store.state.onlineConsultations.answer = ['CHOICE_1'];
        const component = mountComponent({
          $store,
        });

        // Act
        component.setData({ demographicsAnswer: ['CHOICE_2'] });

        // Assert
        expect(component.vm.$store.dispatch).toHaveBeenCalledTimes(1);
        expect(component.vm.$store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswer', ['CHOICE_2']);
      });
    });
  });
  describe('methods', () => {
    describe('demographicsContinueClicked', () => {
      afterEach(() => {
        EventBus.$emit.mockClear();
        NativeApp.resetPageFocus.mockClear();
      });
      each([{
        option: 'NHSAPP_DEMOGRAPHICS_CONSENT_GIVEN',
        consentGiven: true,
      }, {
        option: 'ANOTHER_NON_CONSENT_OPTION',
        consentGiven: false,
      }]).it('will update store consent given if NHSAPP_DEMOGRAPHICS_CONSENT_GIVEN selected and dispatch getServiceDefinition action', async ({ option, consentGiven }) => {
        // Arrange
        const $store = defaultStore();
        $store.state.onlineConsultations.answer = [option];
        const component = mountComponent({ $store });

        // Act
        await component.vm.demographicsContinueClicked();

        // Assert
        expect($store.dispatch).toHaveBeenCalledTimes(2);
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setDemographicsConsentGiven', consentGiven);
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/getServiceDefinition', {
          provider: 'stubs',
          serviceDefinitionId: 'NHS_ADMIN',
        });
      });
      each([
        true,
        false,
      ]).it('will reset page focus when clicked', async (isNative) => {
        // Arrange
        document.activeElement.blur = jest.fn();
        const scrollTo = createScrollTo();
        const $store = defaultStore();
        $store.state.device.isNativeApp = isNative;
        const component = mountComponent({ $store });

        // Act
        await component.vm.demographicsContinueClicked();

        // Assert
        expect(document.activeElement.blur).toHaveBeenCalled();
        if (!isNative) {
          expect(EventBus.$emit).toHaveBeenCalledWith(FOCUS_NHSAPP_ROOT);
        } else {
          expect(NativeApp.resetPageFocus).toHaveBeenCalled();
        }
        expect(scrollTo).toHaveBeenCalledWith(0, 0);
        expect(scrollTo).toHaveBeenCalledTimes(1);
      });
    });
  });
  describe('template', () => {
    it('have a slot for question text, and place in the Question component', () => {
      // Arrange
      const component = mountComponent({
        slots: {
          default: '<p class="q-text">This is the question text</p>',
        },
      });

      // Act
      const questionText = component.find('question-stub > .demographicsQuestion > p.q-text');

      // Assert
      expect(questionText).toBeDefined();
      expect(questionText.text()).toEqual('This is the question text');
    });
    it('will use a QuestionMultipleChoice for presenting the choice option', () => {
      // Arrange
      const component = mountComponent();

      // Act
      const multipleChoice = component.find('question-multiple-choice-stub').vm;

      // Assert
      expect(multipleChoice.options).toEqual([{
        label: 'I consent',
        code: 'NHSAPP_DEMOGRAPHICS_CONSENT_GIVEN',
        required: false,
      }]);
      expect(multipleChoice.name).toEqual('NHSAPP_DEMOGRAPHICS_CONSENT');
    });
    it('will wrap the question multiple choice with a nojsform with a hidden input to identify consent question on postback', () => {
      // Arrange
      const component = mountComponent();

      // Act
      const noJsForm = component.find('no-js-form-stub');
      const hiddenInput = noJsForm.find('input[type=hidden]').element;

      // Assert
      expect(noJsForm.find('question-multiple-choice-stub')).toBeDefined();
      expect(hiddenInput.name).toEqual('ANSWERING_CONSENT_QUESTION');
      expect(hiddenInput.value).toEqual('true');
    });
    it('will have a continue button with demographicsContinueClicked method as click handler', () => {
      // Arrange
      const demographicsContinueClicked = jest.fn();
      const component = mountComponent({
        shallow: false,
        methods: {
          demographicsContinueClicked,
        },
      });

      // Act
      const continueButton = component.find(GenericButton);
      continueButton.trigger('click');

      // Assert
      expect(continueButton.text()).toEqual('translate_onlineConsultations.orchestrator.continueButton');
      expect(demographicsContinueClicked).toHaveBeenCalledTimes(1);
    });
  });
});
