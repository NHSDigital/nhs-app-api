import Withdrawn from '@/pages/organ-donation/withdrawn';
import { $t, createStore, mount } from '../../helpers';

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
    expect($t).toHaveBeenCalledWith('organDonation.withdrawn.messageTextItems');
  });

  it('will show the what next header text', () => {
    expect(wrapper.text()).toContain('translate_organDonation.withdrawn.whatNext.header');
  });

  it('will translate the what next body text', () => {
    expect($t).toHaveBeenCalledWith('organDonation.withdrawn.whatNext.bodyItems');
  });

  describe('created', () => {
    it('will dispatch the "organDonation/init" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/init');
    });
  });
});
