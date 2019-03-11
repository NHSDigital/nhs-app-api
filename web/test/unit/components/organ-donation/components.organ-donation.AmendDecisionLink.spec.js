import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import { createStore, mount } from '../../helpers';

describe('amend decision link', () => {
  let wrapper;
  let $store;
  let $style;

  const mountAmendDecision = () => mount(AmendDecisionLink, { $store, $style });

  beforeEach(() => {
    $style = {
      description: 'desc',
    };
    $store = createStore();
    wrapper = mountAmendDecision();
  });

  describe('link', () => {
    let link;

    beforeEach(() => {
      link = wrapper.find('a');
    });

    it('will display text from organDonation.links.amendDecisionText', () => {
      expect(link.text()).toEqual('translate_organDonation.links.amendDecisionText');
    });

    it('will dispatch the "organDonation/amendDecision" action when clicked', () => {
      link.trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendStart');
    });
  });
});
