<script>
import get from 'lodash/fp/get';
import { ORGAN_DONATION } from '@/lib/routes';
import { DECISION_NOT_FOUND, DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';

const getDecision = get('state.organDonation.registration.decision');
const getAllOrgans = get('state.organDonation.registration.decisionDetails.all');

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

export const EnsureAllOrgansDecision = {
  fetch({ redirect, store }) {
    EnsureOptInDecision.fetch({ redirect, store });
    redirectIfFalse({ redirect, value: getAllOrgans(store) });
  },
};

export default {
  fetch({ redirect, store }) {
    redirectIfFalse({ redirect, value: getDecision(store) !== DECISION_NOT_FOUND });
  },
};
</script>
