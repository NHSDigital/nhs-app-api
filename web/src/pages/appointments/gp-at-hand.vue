<template>
  <div v-if="showTemplate">
    <gp-at-hand-content
      header-tag="gpAtHand.appointments.headerTag"
      content-tag="gpAtHand.appointments.contentTag"/>
  </div>
</template>

<script>
import GpAtHandContent from '@/components/GpAtHandContent';

export default {
  name: 'GpAtHandPage',
  components: {
    GpAtHandContent,
  },
};
</script>
