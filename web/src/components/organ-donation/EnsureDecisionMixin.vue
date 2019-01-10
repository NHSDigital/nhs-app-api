<script>
import get from 'lodash/fp/get';
import { ORGAN_DONATION } from '@/lib/routes';
import { DECISION_NOT_FOUND } from '@/store/modules/organDonation/mutation-types';

export default {
  fetch({ redirect, store }) {
    if (get('state.organDonation.registration.decision')(store) === DECISION_NOT_FOUND) {
      redirect(ORGAN_DONATION.path);
    }
  },
};
</script>
