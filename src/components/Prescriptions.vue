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
      <floating-button-bottom @on-click="onRepeatPrescriptionButtonClicked">
        {{ $t('prescriptions.orderRepeatPrescriptionButton') }}
      </floating-button-bottom>
    </main>
  </div>
</template>

<script>

import Spinner from '@/components/Spinner';
import FloatingButtonBottom from '@/components/FloatingButtonBottom';

export default {
  components: {
    Spinner,
    FloatingButtonBottom,
  },
  mounted() {
    this.$store.dispatch('prescriptions/load', this.$config);
  },
  methods: {
    getButtonContainerClass() {
      return this.$store.state.device.isNativeApp ? 'button-container-native' : 'button-container';
    },
    onRepeatPrescriptionButtonClicked() {
      this.$router.push('repeat-prescription-courses');
    },
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/elements';
</style>
