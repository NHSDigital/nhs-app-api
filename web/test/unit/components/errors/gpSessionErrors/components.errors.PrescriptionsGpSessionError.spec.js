import PrescriptionsGpSessionError from '@/components/errors/gp-session-errors/PrescriptionsGpSessionError';
import MenuItem from '@/components/MenuItem';
import { mount, create$T } from '../../../helpers';

describe('PrescriptionsGPSessionError', () => {
  let content;
  let wrapper;
  const mountWrapper = () => mount(PrescriptionsGpSessionError, {
    $store: {
      state: {
        device: {
          isNativeApp: false,
        },
      },
      $env: {
        EMERGENCY_PRESCRIPTIONS_URL: 'https://www.google.com',
        CONTACT_US_URL: 'https://contact.us',
      },
      dispatch: jest.fn(),
    },
    $t: create$T(),
  });

  beforeEach(() => {
    wrapper = mountWrapper();
    content = wrapper.findAll('p');
  });
  it('will contain the correct paragraph content', () => {
    expect(content.at(0).text()).toContain('translate_gpSessionErrors.prescriptions.ifTheProblemContinues');
  });

  it('will contain the correct emergency prescriptions menuItem', () => {
    const menuItems = wrapper.findAll(MenuItem);
    expect(menuItems.length).toBe(1);
    expect(wrapper.find('#emergency_prescription').exists()).toBe(true);
  });

  it('will contain the 111 link and call 111 content', () => {
    expect(content.at(0).text()).toContain('translate_gpSessionErrors.nhs111Link');
    expect(content.at(0).text()).toContain('translate_gpSessionErrors.orCall');
  });
});
