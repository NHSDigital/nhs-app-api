<template>
  <div v-if="showTemplate">
    <gp-at-hand-content
      :header-tag="'gp_at_hand.appointments.headerTag'"
      :content-tag="'gp_at_hand.appointments.contentTag'" />
  </div>
</template>

<script>
import GpAtHandContent from '@/components/GpAtHandContent';

export default {
  layout: 'nhsuk-layout',
  components: {
    GpAtHandContent,
  },
};
</script>

<style module lang="scss" scoped>

</style>
