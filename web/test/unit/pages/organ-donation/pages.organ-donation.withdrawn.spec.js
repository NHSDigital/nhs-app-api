import Withdrawn from '@/pages/organ-donation/withdrawn';
import { locale, createStore, mount } from '../../helpers';

describe('withdrawn', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
      },
    });
    wrapper = mount(Withdrawn, { $store });
  });

  it('will show the decision withdrawn dialog text', () => {
    expect(wrapper.text()).toContain('translate_organDonation.withdrawn.dialogText');
  });

  it('will translate the message text', () => {
    const items = locale.organDonation.withdrawn.messageTextItems;
    wrapper = mount(Withdrawn, {
      state: {
        device: {
          source: 'web',
        },
      },
    });

    items.forEach(item => expect(wrapper.text()).toContain(item));
  });

  it('will show the what next header text', () => {
    expect(wrapper.text()).toContain('translate_organDonation.withdrawn.whatNext.header');
  });

  it('will translate the body text', () => {
    const items = locale.organDonation.withdrawn.whatNext.bodyItems;
    items.forEach(item => expect(wrapper.text()).toContain(item));
  });

  describe('created', () => {
    it('will dispatch the "organDonation/init" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/init');
    });
  });
});
