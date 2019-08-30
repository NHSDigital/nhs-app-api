<template>
  <div class="pull-content">
    <symptoms-check/>
  </div>
</template>

<script>
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  components: {
    symptomsCheck,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>

<style module lang="scss" scoped>

</style>
