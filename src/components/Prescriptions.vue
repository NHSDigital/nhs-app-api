<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <div class="info">
        <p>
          <b>{{ $t('prescriptions.noPrescriptionsAvailable.title') }}</b>
        </p>
        <p>
          {{ $t('prescriptions.noPrescriptionsAvailable.contactGp') }}
        </p>
        <p>
          {{ $t('prescriptions.noPrescriptionsAvailable.orderRepeatPrescription') }}
        </p>
      </div>
      <div v-bind:class="getButtonContainerClass()">
        <router-link to="repeat-prescription-courses" tag="button" class="button">
          {{ $t('prescriptions.orderRepeatPrescriptionButton') }}
        </router-link>
      </div>
    </main>
  </div>
</template>

<script>

import Spinner from '@/components/Spinner';

export default {
  components: {
    Spinner,
  },
  mounted() {
    this.$store.dispatch('prescriptions/load', this.$config);
  },
  methods: {
    getButtonContainerClass() {
      return this.$store.state.device.isNativeApp ? 'button-container-native' : 'button-container';
    },
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/elements';
  @import '../style/buttons';
</style>
