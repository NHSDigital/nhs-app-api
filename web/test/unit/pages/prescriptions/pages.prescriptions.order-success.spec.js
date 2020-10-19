import OrderSuccess from '@/pages/prescriptions/order-success';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import { InternetPharmacy, CommunityPharmacy } from '@/lib/pharmacy-detail/pharmacy-sub-types';
import { mount, createStore, normaliseWhiteSpace } from '../../helpers';

const selectedPrescriptions = [{
  id: 'prescription-1',
  name: 'Calpol',
  details: '3ml 4 times daily',
}, {
  id: 'prescription-2',
  name: 'Ibuprofen',
  details: '2 tablets twice daily',
}, {
  id: 'prescription-3',
  name: 'Loratadine',
  details: '1 tablet daily',
}];

let wrapper;
let pharmacy;

const mountPage = ({
  proxy = false,
  hasNoNomPharm = false,
  nomPharmEnabled = false,
  sjrNomPharmEnabled = false,
  subType = undefined,
  url = undefined,
  phone = undefined,
  givenName = undefined,
} = {}) => {
  pharmacy = {
    pharmacyName: 'Peter\'s Pharmacy',
    pharmacySubType: subType,
    url,
    telephoneNumber: phone,
  };

  const $store = createStore({
    getters: {
      'nominatedPharmacy/hasNoNominatedPharmacy': hasNoNomPharm,
      'nominatedPharmacy/nominatedPharmacyEnabled': nomPharmEnabled,
      'repeatPrescriptionCourses/selectedPrescriptions': selectedPrescriptions,
      'serviceJourneyRules/nominatedPharmacyEnabled': sjrNomPharmEnabled,
      'session/isProxying': proxy,
    },
    state: {
      nominatedPharmacy: { pharmacy },
      linkedAccounts: { actingAsUser: { givenName } },
    },
  });

  wrapper = mount(OrderSuccess, {
    $store,
  });
};

const getPrescriptionInfoFromSummaries = summaries => (
  summaries.wrappers.map((summary) => {
    const prescriptionInfo = summary.findAll('p').wrappers;
    return {
      id: summary.element.id,
      name: prescriptionInfo[0].text(),
      details: prescriptionInfo[1].text(),
    };
  })
);

describe('prescriptions order success', () => {
  describe('order summary', () => {
    it('will display the order summary in a card', () => {
      mountPage();

      expect(wrapper.find('p').text()).toEqual('You have ordered:');
      expect(getPrescriptionInfoFromSummaries(wrapper.findAll('[data-purpose=prescription-summary]')))
        .toEqual(selectedPrescriptions);
    });

    it('will not display an order summary when proxying', () => {
      mountPage({ proxy: true });

      expect(wrapper.find('[data-purpose=order-success-summary]').exists()).toBe(false);
    });

    describe('pre-text', () => {
      it('will display you have ordered, when not in proxy', () => {
        mountPage();

        expect(wrapper.find('p').text()).toEqual('You have ordered:');
      });

      it('will display who the order was on behalf of', () => {
        mountPage({ proxy: true, givenName: 'Peter Parket' });

        expect(wrapper.find('p').text()).toEqual('You have ordered a prescription on behalf of Peter Parket.');
      });
    });
  });

  describe('what happens next details', () => {
    let whatHappensNext;

    it('will have a h2 for what happens next', () => {
      mountPage();

      expect(wrapper.find('h2').text()).toEqual('What happens next');
    });

    describe('when proxying', () => {
      it('will display the order will be updated once reviewed', () => {
        mountPage({ proxy: true, givenName: 'Peter' });
        whatHappensNext = wrapper.find('[data-purpose=what-happens-next-proxy]');

        expect(whatHappensNext.find('p').text())
          .toEqual('The order status will be updated once it has been reviewed by Peter\'s GP.');
      });

      it('will have a switch profile button', () => {
        mountPage({ proxy: true });
        whatHappensNext = wrapper.find('[data-purpose=what-happens-next-proxy]');

        expect(whatHappensNext.find(SwitchProfileButton).exists()).toBe(true);
      });
    });

    describe('when not proxying', () => {
      it('will display that it has been sent to my surgery', () => {
        mountPage();
        whatHappensNext = wrapper.find('[data-purpose=what-happens-next]');

        expect(whatHappensNext.find('p').text()).toEqual('Your prescription request has been sent to your GP surgery.');
      });

      describe('nominated pharmacy is enabled but has no nominated pharmacy', () => {
        it('will display the need to collect from surgery', () => {
          mountPage({ hasNoNomPharm: true, nomPharmEnabled: true, sjrNomPharmEnabled: true });

          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-no-nom-pharm]');

          expect(whatHappensNext.find('p').text())
            .toEqual('Once they approve your prescription, you\'ll need to collect it from your GP surgery.');
        });
      });

      describe('has high street pharmacy', () => {
        beforeEach(() => {
          mountPage({
            subType: CommunityPharmacy,
            nomPharmEnabled: true,
            sjrNomPharmEnabled: true,
          });
          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-high-street-pharm]');
        });

        it('will display what happens once approved', () => {
          expect(whatHappensNext.find('p').text())
            .toEqual('Once a GP approves it, they\'ll send your prescription to your nominated pharmacy, Peter\'s Pharmacy.');
        });

        it('will display a summary of the pharmacy details', () => {
          const pharmacySummary = whatHappensNext.find(PharmacySummary);

          expect(pharmacySummary.exists()).toBe(true);
          expect(pharmacySummary.vm.pharmacy).toEqual(pharmacy);
          expect(pharmacySummary.vm.pharmacyNameAsHeader).toBe(false);
        });
      });

      describe('has online only pharmacy', () => {
        beforeEach(() => {
          mountPage({ subType: InternetPharmacy, nomPharmEnabled: true, sjrNomPharmEnabled: true });
          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-online-only-pharm]');
        });

        it('will display the order will be sent to nominated pharmacy once approved', () => {
          expect(whatHappensNext.find('p').text())
            .toEqual('Once a GP approves it, they\'ll send your prescription to your nominated pharmacy, Peter\'s Pharmacy.');
        });

        it('will advise on regsitering online or by phone if pharmacy has url and phone number', () => {
          mountPage({ subType: InternetPharmacy, nomPharmEnabled: true, sjrNomPharmEnabled: true, url: 'https://test.abc', phone: '07777777777' });
          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-online-only-pharm]');

          const advice = whatHappensNext.findAll('p').at(1);
          const adviceUrl = advice.find(AnalyticsTrackedTag);

          expect(normaliseWhiteSpace(advice.text()))
            .toEqual('You may need to register with Peter\'s Pharmacy separately at test.abc or call them at 07777777777.');
          expect(adviceUrl.vm.text).toEqual('test.abc');
          expect(adviceUrl.vm.href).toEqual('https://test.abc');
        });

        it('will advise on regsitering online if pharmacy only has url', () => {
          mountPage({ subType: InternetPharmacy, nomPharmEnabled: true, sjrNomPharmEnabled: true, url: 'https://test.abc' });
          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-online-only-pharm]');

          const advice = whatHappensNext.findAll('p').at(1);
          const adviceUrl = advice.find(AnalyticsTrackedTag);

          expect(normaliseWhiteSpace(advice.text()))
            .toEqual('You may need to register with Peter\'s Pharmacy separately at test.abc.');
          expect(adviceUrl.vm.text).toEqual('test.abc');
          expect(adviceUrl.vm.href).toEqual('https://test.abc');
        });

        it('will advise on regsitering by if pharmacy only has telephone number', () => {
          mountPage({ subType: InternetPharmacy, nomPharmEnabled: true, sjrNomPharmEnabled: true, phone: '07777777777' });
          whatHappensNext = wrapper.find('[data-purpose=what-happens-next]').find('[data-purpose=what-happens-next-online-only-pharm]');

          expect(normaliseWhiteSpace(whatHappensNext.findAll('p').at(1).text()))
            .toEqual('You may need to register with Peter\'s Pharmacy separately by calling them at 07777777777.');
        });

        it('will advise on what will happen when the prescription is ready', () => {
          expect(whatHappensNext.findAll('p').at(2).text())
            .toEqual('When the pharmacists have checked and prepared your prescription, Peter\'s Pharmacy will send it to you in the post.');
        });
      });

      describe('has multiple pharmacies, is at a dispensing practice or both, or EPS is not available', () => {
        it('will advise that the status will be update when reviewed by GP', () => {
          mountPage();

          whatHappensNext = wrapper.find('[data-purpose=what-happens-next-default]');

          expect(whatHappensNext.text()).toEqual('The order status will be updated once it has been reviewed by your GP.');
        });
      });
    });
  });
});
