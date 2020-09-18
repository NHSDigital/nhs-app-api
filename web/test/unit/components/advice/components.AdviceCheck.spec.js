import i18n from '@/plugins/i18n';
import MenuItem from '@/components/MenuItem';
import AdviceCheck from '@/components/advice/AdviceCheck';
import * as dependency from '@/lib/utils';
import { APPOINTMENT_GP_ADVICE_PATH } from '@/router/paths';
import { createStore, mount, createRouter, createEvent } from '../../helpers';

const createHttp = (rules = undefined) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(() => Promise.resolve(rules)),
});
dependency.redirectTo = jest.fn();

describe('GP Guidence button tests', () => {
  let $store;
  let $router;

  const createWrapper = (
    rules,
    isLoggedIn,
    cdssAdviceEnabled,
    isProofLevel9 = true,
    knownServiceId = 'engage',
  ) => {
    $router = createRouter();
    $store = createStore({
      $router,
      dispatch: jest.fn(),
      state: {
        device: {
          isNativeApp: false,
        },
        serviceJourneyRules: rules,
        knownServices: {
          knownServices: [{
            id: knownServiceId,
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'session/isLoggedIn': isLoggedIn,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
        'session/isProofLevel9': isProofLevel9,
        'serviceJourneyRules/silverIntegrationEnabled': () => (true),
      },
    });
    return mount(AdviceCheck, { $store, $router, mountOpts: { i18n } });
  };

  describe('Visability Tests', () => {
    let wrapper;
    beforeEach(() => {
      wrapper = createWrapper({
        loggedIn: {
          provider: 'eConsult',
        },
      }, true, true);
    });

    it('will contain the correct content ', async () => {
      const tagArray = wrapper.findAll(MenuItem);
      expect(tagArray.length).toBe(5);

      const coronaCheckerButtonHeader = tagArray.at(0).find('li a span h2');
      expect(coronaCheckerButtonHeader.text()).toContain('Get advice about coronavirus');

      const coronaCheckerButtonText = tagArray.at(0).find('li a span p');
      expect(coronaCheckerButtonText.text()).toContain('Find out what to do if you think you have coronavirus');

      const conditionsCheckerButtonHeader = tagArray.at(1).find('li a span h2');
      expect(conditionsCheckerButtonHeader.text()).toContain('Search conditions and treatments');

      const conditionsCheckerButtonText = tagArray.at(1).find('li a span p');
      expect(conditionsCheckerButtonText.text()).toContain('Find trusted NHS information on hundreds of conditions');

      const adviceCheckerButtonHeader = tagArray.at(2).find('li a span h2');
      expect(adviceCheckerButtonHeader.text()).toContain('Use NHS 111 online');

      const adviceCheckerButtonText = tagArray.at(2).find('li a span p');
      expect(adviceCheckerButtonText.text()).toContain('Check if you need urgent help and find out what to do next');

      const adviceButtonHeader = tagArray.at(3).find('li a span h2');
      expect(adviceButtonHeader.text()).toContain('Ask your GP for advice');

      const adviceButtonText = tagArray.at(3).find('li a span p');
      expect(adviceButtonText.text()).toContain('Consult your GP through an online form. Your GP surgery will reply by phone or email');

      const engageAdviceButtonHeader = tagArray.at(4).find('li a span h2');
      expect(engageAdviceButtonHeader.text()).toContain('Ask your GP for advice');

      const engageAdviceButtonText = tagArray.at(4).find('li a span p');
      expect(engageAdviceButtonText.text()).toContain('Answer questions online and get a response from your GP surgery');
    });

    it('Hides the GP advice button if logged in but practice doesnt offer it', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, false);
      const adminMenuItem = wrapper.find('#btn_gpAdvice');
      expect(adminMenuItem.exists()).toBe(false);
    });

    it('Shows the GP advice button if logged in and practice does offer it', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, true);
      const adminMenuItem = wrapper.find('#btn_gpAdvice');
      expect(adminMenuItem.exists()).toBe(true);
    });

    it('Hides the GP advice button if not logged in but practice does offer it', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, false, true);
      const adminMenuItem = wrapper.find('#btn_gpAdvice');
      expect(adminMenuItem.exists()).toBe(false);
    });

    it('Hides the GP advice button if not logged in and practice does offer it', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, false, false);
      const adminMenuItem = wrapper.find('#btn_gpAdvice');
      expect(adminMenuItem.exists()).toBe(false);
    });

    it('Hide the GP advice button if logged in, practice does offer it but the user is p5', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, true, false);
      const adminMenuItem = wrapper.find('#btn_gpAdvice');
      expect(adminMenuItem.exists()).toBe(false);
    });

    it('Shows the Engage medical advice button if practice offers it and is proof level 9', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, false, true, 'engage');
      const engageMedicalAdviceMenuItem = wrapper.find('#btn_engage_medical_advice');
      expect(engageMedicalAdviceMenuItem.exists()).toBe(true);
    });

    it('Does not show the Engage medical advice button if practice offers it but is not proof level 9', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, false, false, 'engage');
      const engageMedicalAdviceMenuItem = wrapper.find('#btn_engage_medical_advice');
      expect(engageMedicalAdviceMenuItem.exists()).toBe(false);
    });

    it('Does not show the Engage medical advice button if practice does not offer it but proof level 9', async () => {
      const mockRules = {
        loggedIn: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules, true, false, true, 'pkb');
      const engageMedicalAdviceMenuItem = wrapper.find('#btn_engage_medical_advice');
      expect(engageMedicalAdviceMenuItem.exists()).toBe(false);
    });

    describe('Physical tests', () => {
      beforeEach(() => {
        wrapper = createWrapper({
          loggedIn: {
            provider: 'eConsult',
          },
        }, true, true);
      });

      it('will navigate to the Preexisting condititons page when clicked', () => {
        const event = createEvent({ currentTarget:
          { pathname: APPOINTMENT_GP_ADVICE_PATH } });
        wrapper.vm.navigate(event);
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_GP_ADVICE_PATH);
      });
    });
  });
});

