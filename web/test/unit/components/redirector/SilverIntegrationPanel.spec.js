import get from 'lodash/fp/get';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

describe('Silver Integration warning panel', () => {
  let wrapper;

  const mountComponent = ({ servicePurchaser = 'Your GP surgery' } = {}) => {
    const locale = {
      thirdPartyProviders: {
        warningConjunctions: {
          heading: 'This service is provided by {providerName}',
          paragraph: '{{ servicePurchaser }} has chosen this {{ serviceType }} provider.',
          button: 'Continue',
          linkText: 'Find out more about {{ serviceTypePlural }}',
        },
        foo: {
          jumpOffs: [
            {
              id: 'appointments',
              jumpOffContent: {
                headerText: 'View appointments',
                descriptionText: 'See your upcoming and past hospital or other appointments',
              },
              thirdPartyWarning: {
                featureName: 'View appointments',
                servicePurchaser,
                serviceType: 'personal health record service',
                serviceTypePlural: 'personal health record services',
                linkHref: 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-privacy-policy/personal-health-records/',
              },
            },
          ],
        },
      },
    };

    return mount(SilverIntegrationPanel, {
      $store: createStore(),
      mocks: {
        $te: key => !!get(key)(locale),
        $t: key => get(key)(locale),
      },
      propsData: {
        knownService: {
          id: 'foo',
          requiresAssertedLoginIdentity: true,
          showThirdPartyWarning: true,
          url: 'http://www.url.com',
        },
        redirectPath: 'https://test.url.com/nhs-login/login?phrPath=/diary/listAppointments.action',
        sessionStorageName: 'sessionName',
        jumpOffId: 'appointments',
      },
    });
  };

  beforeEach(() => {
    EventBus.$emit.mockClear();
    wrapper = mountComponent();
  });

  describe('mounted', () => {
    it('will emit UPDATE_TITLE on EventBus with third party feature name', async () => {
      await wrapper.vm.$nextTick();
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER);
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'View appointments', true);
    });
  });

  describe.each([
    ['no service purchaser provider', null, false],
    ['service purchaser provider', 'Has service purchaser', true],
  ])('paragraph text', (testName, servicePurchaser, result) => {
    const bodyTextId = 'p.nhsuk-body-m';
    let bodyTextParagraph;

    describe(testName, () => {
      beforeEach(() => {
        wrapper = mountComponent({ servicePurchaser });
        bodyTextParagraph = wrapper.find(bodyTextId);
      });

      it(`will${result ? '' : ' not'} exist`, () => {
        expect(bodyTextParagraph.exists()).toBe(result);
      });
    });
  });

  describe('button', () => {
    let continueButton;

    beforeEach(() => {
      continueButton = wrapper.find('a.nhsuk-button');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    describe('on click', () => {
      beforeEach(() => {
        continueButton.trigger('click');
      });

      it('will emit click', () => {
        expect(wrapper.emitted().click).toBeTruthy();
      });

      it('will disable button', () => {
        expect(wrapper.vm.buttonDisabled).toEqual(true);
        expect(wrapper.find('a.nhsuk-button.nhsuk-button--disabled').exists()).toBe(true);
      });
    });
  });
});
