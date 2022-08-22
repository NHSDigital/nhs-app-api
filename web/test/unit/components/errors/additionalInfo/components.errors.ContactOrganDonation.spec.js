import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import i18n from '@/plugins/i18n';
import { mount } from '../../../helpers';

const mountWrapper = () => mount(ContactOrganDonation, {
  state: {
    device: {
      source: 'web',
    },
  },
  mountOpts: { i18n },
});

describe('ContactOrganDonation', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountWrapper();
  });

  describe('text translations', () => {
    it('will display the email label', () => {
      expect(wrapper.text().replace((/ {2}|\r\n|\n|\r/gm), '')).toContain('If you keep seeing this message, email NHSApp.Enquiries@nhsbt.nhs.uk (NHS Blood and Transplant) for help.');
    });

    it('will display the NHSApp Enquiries email', () => {
      expect(wrapper.text()).toContain('NHSApp.Enquiries@nhsbt.nhs.uk');
    });
  });
});
