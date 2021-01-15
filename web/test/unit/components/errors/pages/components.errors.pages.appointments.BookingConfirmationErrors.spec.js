import BookingConfirmationErrors from '@/components/errors/pages/appointments/BookingConfirmationErrors';
import MenuItem from '@/components/MenuItem';
import i18n from '@/plugins/i18n';
import each from 'jest-each';
import { createStore, mount } from '../../../helpers';

describe('BookingConfirmationErrors', () => {
  const mountWrapper = ({
    cdssAdviceEnabled = true,
    cdssAdminEnabled = true,
    silverIntegrationEnabled = true,
    status = 403,
  } = {}) => mount(BookingConfirmationErrors, {
    $store: createStore({
      $env: {
        CONTACT_US_URL: 'https://contact.us',
      },
      state: {
        device: {
          source: 'web',
        },
        errors: {
          hasConnectionProblem: false,
        },
      },
      getters: {
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationEnabled),
      },
    }),
    propsData: {
      error: {
        status,
      },
    },
    mountOpts: { i18n },
    computed: {
      hasConnection() {
        return true;
      },
    },
  });

  describe('Page content', () => {
    describe('with olc access', () => {
      const wrapper = mountWrapper();
      const content = wrapper.findAll('p');
      const menuItems = wrapper.findAll(MenuItem);

      it('will display to the user they are not able to access appointments', () => {
        expect(content.at(0).text()).toEqual('You are not currently able to book and manage GP appointments online.');
      });

      it('will explain to the user how to get urgent help', () => {
        expect(content.at(1).text()).toContain('If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice go to ');
      });

      it('will contain the nhs 111 link text', () => {
        expect(content.at(1).text()).toContain('111.nhs.uk');
      });

      it('will contain the nhs 111 number', () => {
        expect(content.at(1).text()).toContain('or call 111.');
      });

      it('will have 6 menu items', () => {
        expect(menuItems.length).toBe(6);
      });

      it('will have a menu item for gp advice', () => {
        expect(wrapper.find('#btn_gpAdvice').exists()).toBe(true);
      });

      it('will have a menu item for gp admin help', () => {
        expect(wrapper.find('#btn_adminHelp').exists()).toBe(true);
      });

      it('will have a menu item for corona virus advice', () => {
        expect(wrapper.find('#btn_corona').exists()).toBe(true);
      });

      it('will have a menu item for the nhs 111 service', () => {
        expect(wrapper.find('#btn_111').exists()).toBe(true);
      });
    });

    describe('without cdss admin and silverIntegration enabled', () => {
      const wrapper = mountWrapper({
        cdssAdminEnabled: false,
        silverIntegrationEnabled: false,
      });
      const menuItems = wrapper.findAll(MenuItem);

      it('will not show the admin help menu item if admin help is disabled', () => {
        expect(wrapper.find('#btn_adminHelp').exists()).toBe(false);
        expect(menuItems.length).toBe(3);
      });
    });

    describe('without cdss advice and silverIntegration enabled', () => {
      const wrapper = mountWrapper({
        cdssAdviceEnabled: false,
        silverIntegrationEnabled: false,
      });
      const menuItems = wrapper.findAll(MenuItem);

      it('will not show the gp advice menu item if admin help is disabled', () => {
        expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
        expect(menuItems.length).toBe(3);
      });
    });

    describe('with only silverIntegration enabled', () => {
      const wrapper = mountWrapper({
        cdssAdviceEnabled: false,
        cdssAdminEnabled: false,
      });
      const menuItems = wrapper.findAll(MenuItem);

      it('will show the gp advice menu item when silverIntegration is enabled', () => {
        expect(wrapper.find('#btn_gpAdvice').exists()).toBe(true);
        expect(menuItems.length).toBe(4);
      });

      it('will show the admin help menu item when silverIntegration is enabled', () => {
        expect(wrapper.find('#btn_adminHelp').exists()).toBe(true);
        expect(menuItems.length).toBe(4);
      });
    });

    each([
      400,
      409,
      460,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      const wrapper = mountWrapper({ status });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });

    it('will display a server error if there is an unknown status returned', () => {
      const wrapper = mountWrapper({ status: 600 });
      expect(wrapper.find('#error-dialog-unknown').exists()).toBe(true);
    });
  });
});
