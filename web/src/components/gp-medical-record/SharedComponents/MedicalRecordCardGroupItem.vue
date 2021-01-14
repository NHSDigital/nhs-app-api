<template>
  <div :class="$style['nhsuk-card-group__item']">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'MedicalRecordCardGroupItem',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/medical-record-card-group-item";
</style>
