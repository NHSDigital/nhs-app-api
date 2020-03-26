<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $tc('prescriptions.orderSuccess.proxyMessage', null, { name, givenName }) }}
          </p>
          <switch-profile-button />
        </div>
        <div v-else>
          <p>
            {{ $tc('rp05.confirmationMessage') }}
          </p>
          <desktopGenericBackLink
            :path="backPath"
            :button-text="'prescriptions.orderSuccess.back'"
            @clickAndPrevent="backButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import get from 'lodash/fp/get';
import { PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'PrescriptionOrderSuccess',
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.fullName', this),
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName', this),
      backPath: PRESCRIPTIONS.path,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('repeatPrescriptionCourses/completeOrderJourney');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
