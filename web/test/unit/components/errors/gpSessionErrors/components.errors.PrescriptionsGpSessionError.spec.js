import PrescriptionsGpSessionError from '@/components/errors/gp-session-errors/PrescriptionsGpSessionError';
import MenuItem from '@/components/MenuItem';
import { mount } from '../../../helpers';

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
  });

  beforeEach(() => {
    wrapper = mountWrapper();
    content = wrapper.findAll('p');
  });
  it('will contain the correct paragraph content', () => {
    expect(content.at(0).text()).toContain('If the problem continues and you need to get a prescription now, contact your GP surgery directly. For urgent medical advice, go to');
  });

  it('will contain the correct emergency prescriptions menuItem', () => {
    const menuItems = wrapper.findAll(MenuItem);
    expect(menuItems.length).toBe(1);
    expect(wrapper.find('#emergency_prescription').exists()).toBe(true);
  });

  it('will contain the 111 link and call 111 content', () => {
    expect(content.at(0).text()).toContain('111.nhs.uk');
    expect(content.at(0).text()).toContain('or call 111.');
  });
});
