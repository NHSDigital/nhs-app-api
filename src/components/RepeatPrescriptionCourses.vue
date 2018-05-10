<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <div class="info">
        <p>
          <b>{{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.title') }}</b>
        </p>
        <p>
          {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.contactGp') }}
        </p>
      </div>
      <router-link to="prescriptions" tag="button" class="button grey">
        {{ $t('prescriptions.backToYourPrescriptionsButton') }}
      </router-link>
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
    this.$store.dispatch('repeatPrescriptionCourses/load', this.$config);
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/elements';
  @import '../style/buttons';
</style>
