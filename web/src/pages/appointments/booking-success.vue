<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $tc('appointments.bookingSuccess.proxyMessage', null, { name }) }}
          </p>
          <switch-profile-button />
        </div>
        <div v-else>
          <!-- main user booked content -->
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import get from 'lodash/fp/get';

export default {
  layout: 'nhsuk-layout',
  components: {
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.name', this),
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
