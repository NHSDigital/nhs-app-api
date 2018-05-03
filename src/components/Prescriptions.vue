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
    </main>
  </div>
</template>

<script>

import Spinner from '@/components/Spinner';

export default {
  components: {
    Spinner,
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/elements';
</style>
