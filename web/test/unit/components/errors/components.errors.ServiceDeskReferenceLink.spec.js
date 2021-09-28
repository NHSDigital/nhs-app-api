import ServiceDeskReferenceLink from '@/components/errors/ServiceDeskReferenceLink';
import { mount } from '../../helpers';

describe('ServiceDeskReferenceLink', () => {
  let wrapper;

  const mountWrapper = ({ apiErrors }) => mount(ServiceDeskReferenceLink, {
    $store: {
      state: {
        device: {
          isNativeApp: true,
        },
        errors: {
          apiErrors,
        },
      },
      $env: {
        CONTACT_US_URL: 'http://stubs.local.bitraft.io/contact-us',
      },
    },
  });

  describe('ServiceDeskReferenceLink', () => {
    describe('content', () => {
      let link;

      beforeEach(() => {
        wrapper = mountWrapper({
          apiErrors: [
            {
              serviceDeskReference: 'h438t9',
            },
          ],
        });
        link = wrapper.find('a');
      });
      it('will populate the text with the correct errorCode', () => {
        expect(link.text()).toBe('Contact us if you keep seeing this message, quoting error code h438t9');
      });

      it('will compute the correct aria-label', () => {
        expect(link.attributes('aria-label')).toBe('Contact us if you keep seeing this message, quoting error code h,4,3,8,t,9');
      });
    });

    describe('contact us', () => {
      let link;

      beforeEach(() => {
        global.open = jest.fn();
        wrapper = mountWrapper({
          apiErrors: [
            {
              serviceDeskReference: '6b5276',
            },
          ],
        });

        link = wrapper.find('a');
        link.trigger('click');
      });

      it('will open the correct link and append the errorCode', () => {
        expect(global.open).toBeCalledWith('http://stubs.local.bitraft.io/contact-us?errorcode=6b5276', '');
      });
    });

    describe('no apiErrors', () => {
      beforeEach(() => {
        wrapper = mountWrapper({
          apiErrors: [],
        });
      });

      it('will return an empty string for the errorCode', () => {
        expect(wrapper.vm.errorCode).toBe('');
      });
    });
  });
});
