<template>
  <div class="pull-content">
    <symptoms-check/>
  </div>
</template>

<script>
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  layout: 'nhsuk-layout',
  components: {
    symptomsCheck,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>

