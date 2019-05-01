import BackButton from '@/components/BackButton';
import MoreAboutOrgans from '@/pages/organ-donation/more-about-organs';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

const createState = () => ({
  organDonation: initialState(),
  device: {
    isNativeApp: true,
  },
});

describe('organ donation more about organs', () => {
  let $store;
  let wrapper;

  const mountMoreAboutOrgans = () => mount(MoreAboutOrgans, {
    $store,
    t: (key) => {
      if (key === 'organDonation.moreAboutOrgans.contentItems') {
        return [
          { subheader: 'Heart', body: 'Cardiac' },
          { subheader: 'Lungs', body: 'Pulmonary' },
        ];
      }
      return $t(key);
    },
  });

  beforeEach(() => {
    $store = createStore({ state: createState() });
    wrapper = mountMoreAboutOrgans();
  });

  it('will show the header text', () => {
    expect(wrapper.find('h2').text()).toEqual('translate_organDonation.moreAboutOrgans.header');
  });

  describe('content items', () => {
    let subheaders;
    let bodies;

    beforeEach(() => {
      subheaders = wrapper.findAll('h3');
      bodies = wrapper.findAll('p');
    });

    it('will add each content item with a subheader', () => {
      expect(subheaders.length).toBe(2);
      expect(subheaders.at(0).text()).toBe('Heart');
      expect(subheaders.at(1).text()).toBe('Lungs');
    });

    it('will add each content item with a body', () => {
      expect(bodies.length).toBe(2);
      expect(bodies.at(0).text()).toBe('Cardiac');
      expect(bodies.at(1).text()).toBe('Pulmonary');
    });
  });

  describe('back', () => {
    describe('button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find(BackButton);
      });

      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });
    });
  });
});
