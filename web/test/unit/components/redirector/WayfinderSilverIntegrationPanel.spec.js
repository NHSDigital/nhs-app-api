import get from 'lodash/fp/get';
import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import { createStore, mount } from '../../helpers';

describe('Wayfinder Silver Integration warning panel', () => {
  let wrapper;
  let locale;

  const mountComponent = () => {
    locale = {
      thirdPartyProviders: {
        warningConjunctions: {
          paragraph: '{{ servicePurchaser }} has chosen this {{ serviceType }} provider.',
          button: 'Continue',
          linkText: 'Find out more about {{ serviceTypePlural }}',
        },
        wayfinder: {
          id: 'wayfinder',
          jumpOffContent: {
            headerText: 'View or manage appointment',
            descriptionText: '',
          },
          wayfinderJumpOffs: {
            drDoctor: {
              serviceId: 'drDoctor',
              providerName: 'DrDoctor',
            },
          },
          thirdPartyWarning: {
            featureName: 'View or manage appointment',
            servicePurchaser: 'Your hospital',
            serviceType: 'personal health record service',
            serviceTypePlural: 'personal health record services',
            linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
          },
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
          id: 'drDoctor',
          requiresAssertedLoginIdentity: true,
          showThirdPartyWarning: true,
          url: 'http://www.url.com',
        },
        redirectPath: 'https://test.url.com/nhs-login/login?phrPath=/diary/listAppointments.action',
        sessionStorageName: 'sessionName',
        jumpOffId: 'appointments',
        isWayfinderUrl: true,
      },
    });
  };

  beforeEach(() => {
    wrapper = mountComponent();
  });

  describe('providerName', () => {
    it('will setup the correct wayfinder thirdPartyServiceContent when isWayfinderUrl is true', () => {
      expect(wrapper.vm.buttonDisabled).toBe(false);
      expect(wrapper.vm.thirdPartyServiceContent).toBe(locale.thirdPartyProviders.wayfinder);
      // All wayfinder jumps off are appointments so third party warning content is at the provider
      // level instead of the jump off level.
      expect(wrapper.vm.jumpOffContent).toBe(locale.thirdPartyProviders.wayfinder);
    });
  });

  describe('providerName', () => {
    it('will return the wayfinder provider name when isWayfinderUrl is true', () => {
      expect(wrapper.vm.providerName).toBe('DrDoctor');
    });
  });
});
