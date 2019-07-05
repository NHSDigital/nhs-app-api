import ConfirmPrescription from '@/pages/prescriptions/confirm-prescription-details';
import { createStore, mount } from '../../helpers';

describe('confirm prescriptions', () => {
  const mountPage = ({ hasNoNominatedPharmacy, pharmacyName = undefined, sjrEnabled = true }) => {
    const $store = createStore({
      state: {
        device: {
          source: 'web',
        },
        nominatedPharmacy: {
          pharmacy: {
            pharmacyName,
          },
        },
        repeatPrescriptionCourses: {
          specialRequest: '',
          specialRequestNecessity: 'NotAllowed',
        },
      },
    });
    $store.getters['repeatPrescriptionCourses/selectedPrescriptions'] = [{ courseId: 1 }];
    $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = hasNoNominatedPharmacy;
    $store.getters['serviceJourneyRules/nominatedPharmacyEnabled'] = sjrEnabled;

    return mount(ConfirmPrescription, { $store });
  };

  describe('nominated pharmacy summary', () => {
    const pharmacyBlockId = '#my-nominated-pharmacy';
    let wrapper;

    describe('SJR disabled', () => {
      beforeEach(() => {
        wrapper = mountPage({ hasNoNominatedPharmacy: true, pharmacyName: 'boots', sjrEnabled: false });
      });

      it('will not exist', () => {
        expect(wrapper.find(pharmacyBlockId).exists()).toBe(false);
      });
    });

    describe('SJR enabled', () => {
      const sjrEnabled = true;

      describe('has no nominated pharmacy', () => {
        beforeEach(() => {
          wrapper = mountPage({ hasNoNominatedPharmacy: true, sjrEnabled });
        });

        it('will not exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(false);
        });
      });

      describe('has nominated pharmacy', () => {
        beforeEach(() => {
          wrapper = mountPage({ hasNoNominatedPharmacy: false, pharmacyName: 'Boots', sjrEnabled });
        });

        it('will exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(true);
        });
      });
    });
  });
});
