<template>
  <nhs-uk-layout>
    <nuxt/>
  </nhs-uk-layout>
</template>

<script>
import NhsUkLayout from '@/components/layout/NhsUkLayout';

export default {
  components: {
    NhsUkLayout,
  },
};
</script>

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
  @import "../style/links";
</style>
