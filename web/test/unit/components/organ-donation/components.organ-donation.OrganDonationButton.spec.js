/* eslint-disable object-curly-newline */
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import { ORGAN_DONATION_ADDITIONAL_DETAILS } from '@/lib/routes';
import { $t, createStore, mount } from '../../helpers';

describe('organ donation button', () => {
  let $router;
  let $store;
  let wrapper;

  beforeEach(() => {
    $router = [];
    $store = createStore();
    wrapper = mount(OrganDonationButton, {
      $router,
      $store,
      propsData: { decision: 'boo' },
    });
  });

  describe('clicked', () => {
    beforeEach(() => {
      wrapper.find('button').trigger('click');
    });

    it('will dispatch `makeDecision` with the supplied decision', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', 'boo');
    });

    it('will push the additional details path to the router', () => {
      expect($router).toContain(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
    });
  });


  describe('text translations', () => {
    it('will display the no button header', () => {
      expect($t).toHaveBeenCalledWith('organDonation.register.noButton.header');
    });

    it('will display the no button subheader', () => {
      expect($t).toHaveBeenCalledWith('organDonation.register.noButton.subheader');
    });
  });

  describe('computed properties', () => {
    describe('formAction', () => {
      it('will be the ORGAN_DONATION path', () => {
        expect(wrapper.vm.formAction).toEqual(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
      });
    });

    describe('noJsValue', () => {
      it('will include the decision', () => {
        expect(wrapper.vm.noJsValue).toEqual({
          organDonation: {
            registration: {
              decision: 'boo',
            },
          },
        });
      });
    });
  });

  describe('props', () => {
    it('will set the form value based on the decision', () => {
      const form = wrapper.find(NoJsForm);
      expect(form.props().value).toEqual({
        organDonation: {
          registration: {
            decision: 'boo',
          },
        },
      });
    });
  });
});
