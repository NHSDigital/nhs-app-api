/* eslint-disable object-curly-newline */
import OrganChoice from '@/components/organ-donation/OrganChoice';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import {
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

const organName = 'heart';

describe('organ choice component', () => {
  let $store;
  let wrapper;
  let yesRadioButton;
  let noRadioButton;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });
  });

  describe('yes selected', () => {
    beforeEach(() => {
      wrapper = mount(OrganChoice, {
        $store,
        $t,
        propsData: { title: 'organDonation.someOrgans.heartTitle', organName: 'heart' },
      });
      yesRadioButton = wrapper.findAll(GenericRadioButton).at(0);
    });

    it('will dispatch `setSomeOrgans` when a radio button is selected', () => {
      wrapper.vm.selected('Yes');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setSomeOrgans', { choice: 'heart', value: 'Yes' });
    });

    it('will have a yes radio button with value set to Yes', () => {
      expect(yesRadioButton.vm.value).toEqual('Yes');
    });

    describe('text translations', () => {
      it('will display the organ title', () => {
        expect($t).toHaveBeenCalledWith('organDonation.someOrgans.heartTitle');
      });
    });

    describe('computed properties', () => {
      describe('currentChoice', () => {
        it('will be the state value for that organ', () => {
          expect(wrapper.vm.currentChoice)
            .toEqual($store.state.organDonation.registration.decisionDetails.choices[organName]);
        });
      });
    });
  });

  describe('no selected', () => {
    beforeEach(() => {
      wrapper = mount(OrganChoice, {
        $store,
        $t,
        propsData: { title: 'organDonation.someOrgans.heartTitle', organName: 'heart' },
      });
      noRadioButton = wrapper.findAll(GenericRadioButton).at(1);
    });

    it('will dispatch `setSomeOrgans` when a radio button is selected', () => {
      wrapper.vm.selected('No');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setSomeOrgans', { choice: 'heart', value: 'No' });
    });

    it('will have a no radio button with value set to No', () => {
      expect(noRadioButton.vm.value).toEqual('No');
    });

    describe('text translations', () => {
      it('will display the organ title', () => {
        expect($t).toHaveBeenCalledWith('organDonation.someOrgans.heartTitle');
      });
    });

    describe('computed properties', () => {
      describe('currentChoice', () => {
        it('will be the state value for that organ', () => {
          expect(wrapper.vm.currentChoice)
            .toEqual($store.state.organDonation.registration.decisionDetails.choices[organName]);
        });
      });
    });
  });
});
