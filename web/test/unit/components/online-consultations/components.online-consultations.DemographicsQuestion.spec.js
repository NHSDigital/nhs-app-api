import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import GenericButton from '@/components/widgets/GenericButton';
import i18n from '@/plugins/i18n';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import each from 'jest-each';
import {
  ONLINE_CONSULTATIONS_PRIVACY_URL,
} from '@/router/externalLinks';
import { mount, shallowMount, createStore, createScrollTo, createRouter } from '../../helpers';

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
      providerName: 'Stubs',
    },
    slots,
    methods,
    $router: createRouter(),
    mountOpts: {
      i18n,
    },
  })
);

describe('demographics question', () => {
  describe('props', () => {
    each([
      'provider',
      'serviceDefinitionId',
    ]).it('is required and is a String', (propName) => {
      const component = mountComponent();
      const prop = component.vm.$options.props[propName];

      expect(prop.required).toBeTruthy();
      expect(prop.type).toBe(String);
    });
  });
  describe('methods', () => {
    describe('demographicsContinueClicked', () => {
      afterEach(() => {
        EventBus.$emit.mockClear();
        NativeApp.resetPageFocus.mockClear();
      });

      it('will update store consent given if isAccepted is true and dispatch getServiceDefinition action', async () => {
        // Arrange
        const $store = defaultStore();
        const component = mountComponent({ $store });

        component.setData({ isDemographicsAccepted: true });

        // Act
        await component.vm.demographicsContinueClicked();

        // Assert
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setDemographicsConsentGiven', true);
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
    describe('selectValueChanged', () => {
      it('will dispatch the correct code when isDemographicsAccepted is true', () => {
        const $store = defaultStore();
        const component = mountComponent({ $store });
        component.setData({ isDemographicsAccepted: false, code: 'DEMOGRAPHICS_CODE' });
        component.vm.selectValueChanged();

        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswer', 'DEMOGRAPHICS_CODE');
      });
      it('will dispatch undefined when isDemographicsAccepted is false', () => {
        const $store = defaultStore();
        const component = mountComponent({ $store });
        component.setData({ isDemographicsAccepted: true });
        component.vm.selectValueChanged();

        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswer', undefined);
      });
    });
  });
  describe('template', () => {
    it('will display a warning containing a link to the online consultations help page', () => {
      // Arrange
      const component = mountComponent();
      // Act
      const warning = component.find('#demographicsWarning');
      const warningHelpLink = warning.find('a');
      // Assert
      expect(warning.text()).toContain('This service is provided by an online consultation service provider, Stubs, on behalf of your GP surgery.');
      expect(warningHelpLink.text()).toEqual('Find out more about online consultation services.');
      expect(warningHelpLink.attributes().href).toEqual(ONLINE_CONSULTATIONS_PRIVACY_URL);
    });
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
    it('will use a GenericCheckbox for presenting the choice option', () => {
      // Arrange
      const component = mountComponent();

      // Act
      const multipleChoice = component.find('generic-checkbox-stub').vm;

      // Assert
      expect(multipleChoice.name).toEqual('NHSAPP_DEMOGRAPHICS_CONSENT');
      expect(multipleChoice.required).toEqual(false);
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
      expect(continueButton.text()).toEqual('Continue');
      expect(demographicsContinueClicked).toHaveBeenCalledTimes(1);
    });
  });
});
