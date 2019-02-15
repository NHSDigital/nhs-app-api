import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import { createStore, mount } from '../../helpers';

describe('amend decision link', () => {
  let wrapper;
  let $store;

  const mountAmendDecisionLink = () => mount(AmendDecisionLink, { $store });

  beforeEach(() => {
    $store = createStore();
    wrapper = mountAmendDecisionLink();
  });

  it('will display text from organDonation.amendDecision.linkText', () => {
    expect(wrapper.text()).toEqual('translate_organDonation.amendDecision.linkText');
  });

  it('will dispatch the "organDonation/amendDecision" action when clicked', () => {
    wrapper.trigger('click');
    expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendStart');
  });
});
