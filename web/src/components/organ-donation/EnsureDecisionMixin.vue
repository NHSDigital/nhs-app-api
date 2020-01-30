<script>
import get from 'lodash/fp/get';
import { DECISION_APPOINTED_REP, DECISION_OPT_IN, DECISION_UNKNOWN } from '@/store/modules/organDonation/mutation-types';
import { INDEX, ORGAN_DONATION } from '@/lib/routes';
import { isNativeApp } from '@/components/NativeOnlyMixin';

const getDecision = get('state.organDonation.registration.decision');
const canAmendDecision = decision => ![DECISION_APPOINTED_REP, DECISION_UNKNOWN].includes(decision);
const redirectIfFalse = ({ redirect, store, value }) => {
  if (!isNativeApp({ store })) {
    redirect(INDEX.path);
  } else if (!value) {
    redirect(ORGAN_DONATION.path);
  }
};

export const EnsureOptInDecision = {
  fetch({ redirect, store }) {
    redirectIfFalse({ redirect, store, value: getDecision(store) === DECISION_OPT_IN });
  },
};

export const EnsureCanSubmit = {
  fetch({ redirect, store }) {
    const decision = getDecision(store);
    const value = canAmendDecision(decision)
      || (decision === DECISION_APPOINTED_REP && get('state.organDonation.isWithdrawing')(store));
    redirectIfFalse({ redirect, store, value });
  },
};

export default {
  fetch({ redirect, store }) {
    redirectIfFalse({ redirect, store, value: canAmendDecision(getDecision(store)) });
  },
};
</script>
