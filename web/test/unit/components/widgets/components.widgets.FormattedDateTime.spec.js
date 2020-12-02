import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import { createRouter, mount } from '../../helpers';

describe('FormattedDateTime', () => {
  let $router;
  let wrapper;

  const mountFormattedDateTime = ({ dateTime, summaryTimeFormat } = {}) =>
    mount(FormattedDateTime, {
      $router,
      propsData: {
        dateTime,
        summaryTimeFormat,
      },
    });

  beforeEach(() => {
    $router = createRouter();
  });

  describe('props', () => {
    describe('dateTime', () => {
      beforeEach(() => {
        wrapper = mountFormattedDateTime({ dateTime: '2019-12-14T14:15:12.356Z' });
      });

      it('will format sent time to `Sent DD MMMM YYYY at h:mma` london time', () => {
        expect(wrapper.text()).toBe('Sent 14 December 2019 at 2:15pm');
      });

      it('will set datetime attribute to `YYYY-MM-DD h:mma` london time', () => {
        expect(wrapper.attributes('datetime')).toBe('2019-12-14 2:15pm');
      });
    });

    describe('summaryTimeFormat', () => {
      beforeEach(() => {
        wrapper = mountFormattedDateTime({ dateTime: '2019-12-14T14:15:12.356Z', summaryTimeFormat: true });
      });

      it('will format sent time to `DD MMMM YYYY` london time', () => {
        expect(wrapper.text()).toBe('14 December 2019');
      });
    });
  });
});
