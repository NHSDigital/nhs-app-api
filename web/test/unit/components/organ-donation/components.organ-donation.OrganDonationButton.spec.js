/* eslint-disable object-curly-newline */
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import { ORGAN_DONATION } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

describe('organ donation button', () => {
  let $store;
  let $t;
  let wrapper;

  beforeEach(() => {
    $store = createStore();
    $t = jest.fn();
    $t.mockReturnValue('translated');
    wrapper = mount(OrganDonationButton, {
      $store,
      $t,
      propsData: { decision: 'boo' },
    });
  });

  it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
    wrapper.find('button').trigger('click');
    expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', 'boo');
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
        expect(wrapper.vm.formAction).toEqual(ORGAN_DONATION.path);
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
