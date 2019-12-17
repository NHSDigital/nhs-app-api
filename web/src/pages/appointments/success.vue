<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <shutter feature="appointments" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3" />
      </div>
    </div>
  </div>
</template>

<script>
import Shutter from '@/components/linked-profiles/Shutter';

export default {
  layout: 'nhsuk-layout',
  components: {
    Shutter,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
