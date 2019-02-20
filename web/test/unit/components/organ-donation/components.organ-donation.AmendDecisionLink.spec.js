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

  it('will display text from organDonation.links.amendText', () => {
    expect(wrapper.text()).toEqual('translate_organDonation.links.amendText');
  });

  it('will dispatch the "organDonation/amendDecision" action when clicked', () => {
    wrapper.trigger('click');
    expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendStart');
  });
});
