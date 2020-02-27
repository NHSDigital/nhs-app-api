<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full"/>
  </div>
</template>

<script>

export default {
  name: 'HospitalAppointments',
  layout: 'nhsuk-layout',
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
