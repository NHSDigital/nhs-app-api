<template>
  <div v-if="showTemplate">

    <gp-at-hand-content
      :header-tag="'gpAtHand.prescriptions.headerTag'"
      :content-tag="'gpAtHand.prescriptions.contentTag'" />

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
