import i18n from '@/plugins/i18n';
import Withdrawn from '@/pages/organ-donation/withdrawn';
import { createStore, mount } from '../../helpers';

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
    wrapper = mount(Withdrawn, { $store, mountOpts: { i18n } });
  });

  it('will show the decision withdrawn dialog text', () => {
    expect(wrapper.text()).toContain('Decision withdrawn');
  });

  it('will translate the message text', () => {
    wrapper = mount(Withdrawn, {
      state: {
        device: {
          source: 'web',
        },
      },
      mountOpts: { i18n },
    });

    expect(wrapper.text()).toContain('You no longer have a decision recorded on the NHS Organ Donor Register.');
    expect(wrapper.text()).toContain('If you die in circumstances where donation is possible, it will be considered that you have agreed to be an organ donor unless you are in an excluded group.');
  });

  it('will show the what next header text', () => {
    expect(wrapper.text()).toContain('What to do next');
  });

  it('will translate the body text', () => {
    expect(wrapper.text()).toContain('Let your family know that you have withdrawn your decision from the register. They will not know what you want unless you tell them.');
  });

  describe('created', () => {
    it('will dispatch the "organDonation/init" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/init');
    });
  });
});
