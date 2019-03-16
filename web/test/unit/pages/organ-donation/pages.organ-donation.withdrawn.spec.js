import Withdrawn from '@/pages/organ-donation/withdrawn';
import { $t, mount } from '../../helpers';

describe('withdrawn', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(Withdrawn, {
      $t,
      state: {
        device: {
          source: 'web',
        },
      },
    });
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
});
