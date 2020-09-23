import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import MenuItem from '@/components/MenuItem';
import each from 'jest-each';
import i18n from '@/plugins/i18n';
import {
  ADVICE_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount, createRouter } from '../../helpers';

jest.mock('@/lib/utils');

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
    return mount(AppointmentGuidanceMenu, { $store, $router, mountOpts: { i18n } });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('Appointment Guidance Menu', () => {
    let wrapper;
    let tagArray;
    let menuItemElement;
    beforeEach(async () => {
      const mockRules = {
        appointments: {
          provider: '',
        },
        cdssAdvice: {
          provider: 'stubs',
        },
        cdssAdmin: {
          provider: 'stubs',
        },
      };
      wrapper = createWrapper(mockRules);
      tagArray = wrapper.findAll(MenuItem);
    });

    describe('Menu item content', () => {
      it('will contain the correct number of items ', () => {
        expect(tagArray.length).toBe(3);
      });

      describe('Get health advice', () => {
        beforeEach(() => {
          menuItemElement = tagArray.at(0);
        });

        it('will contain the correct header content', async () => {
          expect(menuItemElement.find('h2').text()).toContain('Get health advice');
        });

        it('will contain the correct paragraph content', async () => {
          expect(menuItemElement.find('p').text()).toContain('Find information about specific conditions');
        });
      });

      describe('Additional GP services', () => {
        beforeEach(() => {
          menuItemElement = tagArray.at(1);
        });

        it('will contain the correct header content', async () => {
          expect(menuItemElement.find('h2').text()).toContain('Additional GP services');
        });

        it('will contain the correct paragraph content', async () => {
          expect(menuItemElement.find('p').text()).toContain('Get sick notes and GP letters or ask about recent tests');
        });
      });

      describe('Ask your GP for advice', () => {
        beforeEach(() => {
          menuItemElement = tagArray.at(2);
        });

        it('will contain the correct header content', async () => {
          expect(menuItemElement.find('h2').text()).toContain('Ask your GP for advice');
        });

        it('will contain the correct paragraph content', async () => {
          expect(menuItemElement.find('p').text()).toContain('Consult your GP through an online form. Your GP surgery will reply by phone or email');
        });
      });
    });
  });

  describe('menu items', () => {
    let wrapper;

    beforeEach(() => {
      wrapper = createWrapper({
        appointments: { provider: '' },
        cdssAdvice: { provider: 'stubs' },
        cdssAdmin: { provider: 'stubs' },
      });
    });

    each([
      ['advice', 'goToAdvice', '#btn_advice'],
      ['admin help', 'goToAdminHelp', '#btn_gpHelpNoAppointment'],
      ['gp advice', 'goToGpAdvice', '#btn_gpAdvice'],
    ]).it('will set %s clickFunc to %s', (_, clickFunc, id) => {
      expect(wrapper.find(id).vm.clickFunc).toEqual(wrapper.vm[clickFunc]);
    });

    each([
      [ADVICE_PATH, 'goToAdvice'],
      [APPOINTMENT_ADMIN_HELP_PATH, 'goToAdminHelp'],
      [APPOINTMENT_GP_ADVICE_PATH, 'goToGpAdvice'],
    ]).it('will redirect to %s when %s called', (path, method) => {
      wrapper.vm[method]();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, path);
    });

    each([
      [APPOINTMENT_ADMIN_HELP_PATH, 'goToAdminHelp'],
      [APPOINTMENT_GP_ADVICE_PATH, 'goToGpAdvice'],
    ]).it('will setup olc navigation context', (path, method) => {
      wrapper.vm[method]();
      expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setPreviousRoute', APPOINTMENT_BOOKING_GUIDANCE_PATH);
      expect($store.dispatch).toHaveBeenCalledWith('navigation/setNewMenuItem', 1);
      expect($store.dispatch).toHaveBeenCalledWith('navigation/setBackLinkOverride', APPOINTMENT_BOOKING_GUIDANCE_PATH);
    });
  });

  each([
    ['not enabled', false],
    ['enabled', true],
  ]).it('will hide the admin help menu item when cdssAdmin is %s', (_, enabled) => {
    const mockRulesResponse = {
      appointments: { provider: '' },
      cdssAdvice: { provider: 'none' },
      cdssAdmin: { provider: enabled ? 'stubs' : 'none' },
    };
    const adminMenuItem = createWrapper(mockRulesResponse).find('#btn_gpHelpNoAppointment');
    expect(adminMenuItem.exists()).toBe(enabled);
  });

  each([
    ['not enabled', false],
    ['enabled', true],
  ]).it('will hide the gp advice menu item when cdssAdvice is %s', (_, enabled) => {
    const mockRules = {
      appointments: { provider: '' },
      cdssAdvice: { provider: enabled ? 'stubs' : 'none' },
      cdssAdmin: { provider: 'none' },
    };
    const adviceMenuItem = createWrapper(mockRules).find('#btn_gpAdvice');
    expect(adviceMenuItem.exists()).toBe(enabled);
  });
});
