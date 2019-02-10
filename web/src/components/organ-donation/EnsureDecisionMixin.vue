<script>
import get from 'lodash/fp/get';
import { ORGAN_DONATION } from '@/lib/routes';
import { DECISION_UNKNOWN, DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';

const getDecision = get('state.organDonation.registration.decision');

const redirectIfFalse = ({ redirect, value }) => {
  if (!value) {
    redirect(ORGAN_DONATION.path);
  }
};

export const EnsureOptInDecision = {
  fetch({ redirect, store }) {
    redirectIfFalse({ redirect, value: getDecision(store) === DECISION_OPT_IN });
  },
};

export default {
  fetch({ redirect, store }) {
    redirectIfFalse({ redirect, value: getDecision(store) !== DECISION_UNKNOWN });
  },
};
</script>
