import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_NAME } from '@/router/names';

describe('ReloadRecordMixin', () => {
  let next;

  const self = {
    $store: {},
  };
  const from = '';

  beforeEach(() => {
    next = jest.fn();
    self.$store.dispatch = jest.fn();
  });

  const setup = to => ReloadRecordMixin.beforeRouteLeave.apply(
    self,
    [to, from, next],
  );

  it('will not call myRecord/reload when route is not gp medical record index', () => {
    setup({ name: 'random-route-name' });
    expect(self.$store.dispatch).not.toHaveBeenCalled();
  });

  it('will reset myRecord/reload to false when route navigating to is gp medical record index', () => {
    setup({ name: GP_MEDICAL_RECORD_NAME });
    expect(self.$store.dispatch).toHaveBeenCalledWith('myRecord/reload', false);
  });
});
