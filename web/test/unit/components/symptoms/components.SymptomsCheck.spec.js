import SymptomsCheck from '@/components/symptoms/SymptomsCheck';
import { createStore, mount, createRouter, createEvent } from '../../helpers';
import { APPOINTMENT_GP_ADVICE } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';

const createHttp = (rules = undefined) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(() => Promise.resolve(rules)),
});

describe('GP Guidence button tests', () => {
  let $store;
  let $router;

  const createWrapper = (rules, isLoggedIn, cdssAdviceEnabled) => {
    $router = createRouter();
    $store = createStore({
      $router,
      dispatch: jest.fn(),
      state: {
        device: {
          isNativeApp: false,
        },
        serviceJourneyRules: rules,
      },
      getters: {
        'session/isLoggedIn': isLoggedIn,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
      },
    });
    return mount(SymptomsCheck, { $store, $router });
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
      expect(tagArray.length).toBe(4);

      const coronaCheckerButtonHeader = tagArray.at(0).find('li a span h2');
      expect(coronaCheckerButtonHeader.text()).toContain('translate_sy01.corona.subheader');

      const conditionsCheckerButtonHeader = tagArray.at(1).find('li a span h2');
      expect(conditionsCheckerButtonHeader.text()).toContain('translate_sy01.conditionsTreatments.subheader');

      const conditionsCheckerButtonText = tagArray.at(1).find('li a span p');
      expect(conditionsCheckerButtonText.text()).toContain('translate_sy01.conditionsTreatments.body');

      const symptomsCheckerButtonHeader = tagArray.at(2).find('li a span h2');
      expect(symptomsCheckerButtonHeader.text()).toContain('translate_sy01.111.subheader');

      const symptomsCheckerButtonText = tagArray.at(2).find('li a span p');
      expect(symptomsCheckerButtonText.text()).toContain('translate_sy01.111.body');

      const adviceButtonHeader = tagArray.at(3).find('li a span h2');
      expect(adviceButtonHeader.text()).toContain('translate_appointments.guidance.menuItem3.header');

      const adviceButtonText = tagArray.at(3).find('li a span p');
      expect(adviceButtonText.text()).toContain('translate_appointments.guidance.menuItem3.text');
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
          { pathname: APPOINTMENT_GP_ADVICE.path } });
        wrapper.vm.navigate(event);
        expect($router.push).toHaveBeenCalledWith(APPOINTMENT_GP_ADVICE.path);
      });
    });
  });
});

