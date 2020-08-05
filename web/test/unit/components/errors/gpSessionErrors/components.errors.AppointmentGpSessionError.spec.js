import AppointmentsGpSessionError from '@/components/errors/gp-session-errors/AppointmentsGpSessionError';
import MenuItem from '@/components/MenuItem';
import { mount } from '../../../helpers';

describe('Appointment GP session error', () => {
  const mountWrapper = ({
    cdssAdviceEnabled = true,
    cdssAdminEnabled = true,
  } = {}) => mount(AppointmentsGpSessionError, {
    $store: {
      app: {
        $env: {
          CONTACT_US_URL: 'https://contact.us',
        },
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
      dispatch: jest.fn(),
    },
  });

  it('will have the correct error content', () => {
    const wrapper = mountWrapper();
    const content = wrapper.findAll('p');
    const menuItems = wrapper.findAll(MenuItem);

    expect(content.at(0).text()).toEqual('translate_gpSessionErrors.appointments.youCannotBookOnline');
    expect(content.at(1).text()).toContain('translate_gpSessionErrors.appointments.ifTheProblemContinues');
    expect(content.at(1).text()).toContain('translate_gpSessionErrors.nhs111Link');
    expect(content.at(1).text()).toContain('translate_gpSessionErrors.orCall');

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
