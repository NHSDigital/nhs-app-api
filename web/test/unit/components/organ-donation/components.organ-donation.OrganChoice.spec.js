/* eslint-disable object-curly-newline */
import i18n from '@/plugins/i18n';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import {
  initialState, NO, YES,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

const organName = 'heart';

describe('organ choice component', () => {
  let $store;
  let wrapper;

  const mountOrganChoice = () => mount(OrganChoice, {
    $store,
    propsData: {
      title: 'organDonation.organs.heart',
      organName: 'heart',
      showError: false,
    },
    mountOpts: { i18n },
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });
    wrapper = mountOrganChoice();
  });

  describe('radio button', () => {
    let radioButton;
    let label;

    describe('NhsUkRadioGroup ', () => {
      let nhsUkRadioButton;

      beforeEach(() => {
        nhsUkRadioButton = wrapper.find(NhsUkRadioGroup);
      });

      it('should exist', () => {
        expect(nhsUkRadioButton.exists()).toBe(true);
      });

      describe('text translations', () => {
        it('will display the organ title', () => {
          expect(wrapper.text()).toContain('Heart');
        });
      });

      describe('yes', () => {
        beforeEach(() => {
          radioButton = wrapper.find(`#heart-${YES}`);
        });

        it('will have lable Yes', () => {
          label = wrapper.find(`[for="heart-${YES}"]`);
          expect(label.text()).toContain('Yes');
        });

        describe('when clicked', () => {
          beforeEach(() => {
            radioButton.setChecked();
          });

          it('will dispatch `setSomeOrgans` when a radio button is selected', () => {
            expect($store.dispatch).toHaveBeenCalledWith('organDonation/setSomeOrgans', { choice: 'heart', value: 'Yes' });
          });

          describe('computed properties', () => {
            describe('currentChoice', () => {
              it('will be the state value for that organ', () => {
                expect(wrapper.vm.currentChoice)
                  .toEqual($store.state.organDonation.registration
                    .decisionDetails.choices[organName]);
              });
            });
          });
        });
      });

      describe('no', () => {
        beforeEach(() => {
          radioButton = wrapper.find(`#heart-${NO}`);
        });

        it('will have lable No', () => {
          label = wrapper.find(`[for="heart-${NO}"]`);
          expect(label.text()).toContain('No');
        });

        describe('when clicked', () => {
          beforeEach(() => {
            radioButton.setChecked();
          });

          it('will dispatch `setSomeOrgans` when a radio button is selected', () => {
            expect($store.dispatch).toHaveBeenCalledWith('organDonation/setSomeOrgans', { choice: 'heart', value: 'No' });
          });
        });

        describe('computed properties', () => {
          describe('currentChoice', () => {
            it('will be the state value for that organ', () => {
              expect(wrapper.vm.currentChoice)
                .toEqual($store.state.organDonation.registration
                  .decisionDetails.choices[organName]);
            });
          });
        });
      });
    });
  });
});
