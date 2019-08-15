import SymptomsCheck from '@/components/symptoms/SymptomsCheck';
import { createStore, mount, createRouter, createEvent } from '../../helpers';
import { APPOINTMENT_GP_ADVICE } from '@/lib/routes';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

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
      const tagArray = wrapper.findAll(AnalyticsTrackedTag);
      expect(tagArray.length).toBe(3);

      const conditionsCheckerButtonHeader = tagArray.at(0).find('a h2');
      expect(conditionsCheckerButtonHeader.text()).toContain('translate_sy01.a_z.subheader');

      const conditionsCheckerButtonText = tagArray.at(0).find('a p');
      expect(conditionsCheckerButtonText.text()).toContain('translate_sy01.a_z.body');

      const symptomsCheckerButtonHeader = tagArray.at(1).find('a h2');
      expect(symptomsCheckerButtonHeader.text()).toContain('translate_sy01.111.subheader');

      const symptomsCheckerButtonText = tagArray.at(1).find('a p');
      expect(symptomsCheckerButtonText.text()).toContain('translate_sy01.111.body');

      const adviceButtonHeader = tagArray.at(2).find('a h2');
      expect(adviceButtonHeader.text()).toContain('translate_appointments.guidance.menuItem3.header');

      const adviceButtonText = tagArray.at(2).find('a p');
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

