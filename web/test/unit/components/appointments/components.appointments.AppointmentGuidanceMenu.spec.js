import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import { createStore, mount, createRouter, createEvent } from '../../helpers';
import { SYMPTOMS, APPOINTMENT_ADMIN_HELP } from '@/lib/routes';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const createHttp = (rules = undefined) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(() => Promise.resolve(rules)),
});

describe('Appointment guidance menu', () => {
  let $store;
  let $router;
  let cdssAdminEnabled;
  let cdssAdviceEnabled;

  const createWrapper = (rules) => {
    if (rules === undefined) {
      cdssAdminEnabled = false;
      cdssAdviceEnabled = false;
    } else {
      cdssAdviceEnabled = rules.cdssAdvice.provider !== 'none';
      cdssAdminEnabled = rules.cdssAdmin.provider !== 'none';
    }

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
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
      },
    });
    return mount(AppointmentGuidanceMenu, { $store, $router });
  };

  describe('Main Content', () => {
    let wrapper;
    beforeEach(async () => {
      const mockRules = {
        appointments: {
          provider: '',
        },
        cdssAdvice: {
          provider: 'eConsult',
        },
        cdssAdmin: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRules);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules);
    });

    it('will contain the correct content ', async () => {
      const tagArray = wrapper.findAll(AnalyticsTrackedTag);
      expect(tagArray.length).toBe(3);

      const symptomsButtonHeader = tagArray.at(0).find('a h2');
      expect(symptomsButtonHeader.text()).toContain('translate_appointments.guidance.menuItem1.header');

      const symptomsButtonText = tagArray.at(0).find('a p');
      expect(symptomsButtonText.text()).toContain('translate_appointments.guidance.menuItem1.text');

      const requestGPAdviceButtonHeader = tagArray.at(1).find('a h2');
      expect(requestGPAdviceButtonHeader.text()).toContain('translate_appointments.guidance.menuItem2.header');

      const requestGPAdviceButtonText = tagArray.at(1).find('a p');
      expect(requestGPAdviceButtonText.text()).toContain('translate_appointments.guidance.menuItem2.text');

      const requestGPHelpButtonHeader = tagArray.at(2).find('a h2');
      expect(requestGPHelpButtonHeader.text()).toContain('translate_appointments.guidance.menuItem3.header');

      const requestGPHelpButtonText = tagArray.at(2).find('a p');
      expect(requestGPHelpButtonText.text()).toContain('translate_appointments.guidance.menuItem3.text');
    });

    it('will link to the check symptoms page when check symptoms menu item clicked', () => {
      const event = createEvent({ currentTarget: { pathname: SYMPTOMS.path } });
      wrapper.vm.navigate(event);
      expect($router.push).toHaveBeenCalledWith(SYMPTOMS.path);
    });
  });

  it('will hides the request admin help menu item when cdssAdmin is not enabled', async () => {
    const mockRules = {
      appointments: {
        provider: '',
      },
      cdssAdvice: {
        provider: 'none',
      },
      cdssAdmin: {
        provider: 'none',
      },
    };
    const http = createHttp(mockRules);
    const rules = await http.getV1PatientJourneyConfiguration();
    const wrapper = createWrapper(rules);
    const adminMenuItem = wrapper.find('#btn_gpHelpNoAppointment');
    expect(adminMenuItem.exists()).toBe(false);
  });

  it('will only shows the symptoms admin menu item when an unddefined response is returned', async () => {
    const http = createHttp();
    const rules = await http.getV1PatientJourneyConfiguration();
    const wrapper = createWrapper(rules);
    const tagArray = wrapper.findAll(AnalyticsTrackedTag);
    const adminMenuItem = wrapper.find('#btn_gpHelpNoAppointment');
    const adviceMenuItem = wrapper.find('#btn_gpAdvice');

    expect(tagArray.length).toBe(1);
    expect(adminMenuItem.exists()).toBe(false);
    expect(adviceMenuItem.exists()).toBe(false);
  });

  describe('cdssAdmin is enabled', () => {
    let wrapper;
    beforeEach(async () => {
      const mockRulesResponse = {
        appointments: {
          provider: '',
        },
        cdssAdvice: {
          provider: 'none',
        },
        cdssAdmin: {
          provider: 'eConsult',
        },
      };
      const http = createHttp(mockRulesResponse);
      const rules = await http.getV1PatientJourneyConfiguration();
      wrapper = createWrapper(rules);
    });

    it('will shows the request admin help menu item when cdssAdmin is enabled', () => {
      const adminMenuItem = wrapper.find('#btn_gpHelpNoAppointment');
      expect(adminMenuItem.exists()).toBe(true);
    });

    it('will navigate to the admin help orchestrator when admin help menu item clickec', () => {
      const event = createEvent({ currentTarget: { pathname: APPOINTMENT_ADMIN_HELP.path } });
      wrapper.vm.navigate(event);
      expect($store.dispatch).toHaveBeenCalledWith('navigation/setNewMenuItem', 1);
      expect($router.push).toHaveBeenCalledWith(APPOINTMENT_ADMIN_HELP.path);
    });
  });

  it('will hides the gp advice menu item when cdssAdvice is not enabled', async () => {
    const mockRules = {
      appointments: {
        provider: '',
      },
      cdssAdvice: {
        provider: 'none',
      },
      cdssAdmin: {
        provider: 'none',
      },
    };
    const http = createHttp(mockRules);
    const rules = await http.getV1PatientJourneyConfiguration();
    const wrapper = createWrapper(rules);
    const adviceMenuItem = wrapper.find('#btn_gpAdvice');
    expect(adviceMenuItem.exists()).toBe(false);
  });

  it('will shows the gp advice menu item when cdssAdvice is enabled', async () => {
    const mockRules = {
      appointments: {
        provider: '',
      },
      cdssAdvice: {
        provider: 'eConsult',
      },
      cdssAdmin: {
        provider: 'none',
      },
    };
    const http = createHttp(mockRules);
    const rules = await http.getV1PatientJourneyConfiguration();
    const wrapper = createWrapper(rules);
    const adviceMenuItem = wrapper.find('#btn_gpAdvice');
    expect(adviceMenuItem.exists()).toBe(true);
  });
});
