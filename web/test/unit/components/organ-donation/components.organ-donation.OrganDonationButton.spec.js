/* eslint-disable object-curly-newline */
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import NoJsForm from '@/components/no-js/NoJsForm';
import { ORGAN_DONATION_ADDITIONAL_DETAILS, ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import { DECISION_OPT_OUT, DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

describe('organ donation button', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore();
  });

  describe('no state', () => {
    beforeEach(() => {
      wrapper = mount(OrganDonationButton, {
        $store,
        $t,
        propsData: { decision: DECISION_OPT_OUT },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_OUT);
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
                decision: DECISION_OPT_OUT,
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
              decision: DECISION_OPT_OUT,
            },
          },
        });
      });
    });
  });

  describe('yes state', () => {
    beforeEach(() => {
      wrapper = mount(OrganDonationButton, {
        $store,
        $t,
        propsData: { decision: DECISION_OPT_IN },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_IN);
    });

    describe('text translations', () => {
      it('will display the yes button header', () => {
        expect($t).toHaveBeenCalledWith('organDonation.register.yesButton.header');
      });

      it('will display the yes button subheader', () => {
        expect($t).toHaveBeenCalledWith('organDonation.register.yesButton.subheader');
      });
    });

    describe('computed properties', () => {
      describe('formAction', () => {
        it('will be the ORGAN_DONATION_YOUR_CHOICE path', () => {
          expect(wrapper.vm.formAction).toEqual(ORGAN_DONATION_YOUR_CHOICE.path);
        });
      });

      describe('noJsValue', () => {
        it('will include the decision', () => {
          expect(wrapper.vm.noJsValue).toEqual({
            organDonation: {
              registration: {
                decision: DECISION_OPT_IN,
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
              decision: DECISION_OPT_IN,
            },
          },
        });
      });
    });

    describe('data', () => {
      it('will set the data based on the decision', () => {
        const stateData = wrapper.vm;
        expect(stateData.formAction).toEqual(ORGAN_DONATION_YOUR_CHOICE.path);
        expect(stateData.style).toEqual(wrapper.vm.$style['yes-button']);
        expect(stateData.headerKey).toEqual('organDonation.register.yesButton.header');
        expect(stateData.subHeaderKey).toEqual('organDonation.register.yesButton.subheader');
        expect(stateData.noJsValue).toEqual({
          organDonation: {
            registration: {
              decision: DECISION_OPT_IN,
            },
          },
        });
        expect(stateData.icon).toEqual(YesIcon);
      });
    });
  });
});
