import AppointmentsGpSessionError from '@/components/errors/gp-session-errors/AppointmentsGpSessionError';
import MenuItem from '@/components/MenuItem';
import { createStore, mount } from '../../../helpers';

describe('Appointment GP session error', () => {
  const mountWrapper = ({
    cdssAdviceEnabled = true,
    cdssAdminEnabled = true,
  } = {}) => mount(AppointmentsGpSessionError, {
    $store: createStore({
      $env: {
        CONTACT_US_URL: 'https://contact.us',
      },
      state: {
        device: {
          source: 'web',
        },
      },
      getters: {
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
      },
    }),
  });

  it('will have the correct error content', () => {
    const wrapper = mountWrapper();
    const content = wrapper.findAll('p');
    const menuItems = wrapper.findAll(MenuItem);

    expect(content.at(0).text()).toEqual('You are not currently able to book and manage GP appointments online.');
    expect(content.at(1).text()).toContain('If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice go to');
    expect(content.at(1).text()).toContain('111.nhs.uk');
    expect(content.at(1).text()).toContain('or call 111.');

    expect(menuItems.length).toBe(4);
    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(true);
    expect(wrapper.find('#btn_gpHelpNoAppointment').exists()).toBe(true);
    expect(wrapper.find('#btn_corona').exists()).toBe(true);
    expect(wrapper.find('#btn_111').exists()).toBe(true);
  });

  it('will not show the admin help menu item if admin help is disabled', () => {
    const wrapper = mountWrapper({ cdssAdminEnabled: false });
    const menuItems = wrapper.findAll(MenuItem);

    expect(wrapper.find('#btn_gpHelpNoAppointment').exists()).toBe(false);
    expect(menuItems.length).toBe(3);
  });

  it('will not show the gp advice menu item if admin help is disabled', () => {
    const wrapper = mountWrapper({ cdssAdviceEnabled: false });
    const menuItems = wrapper.findAll(MenuItem);

    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
    expect(menuItems.length).toBe(3);
  });
});
