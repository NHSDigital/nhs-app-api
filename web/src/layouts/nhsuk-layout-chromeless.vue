<template>
  <nhs-uk-layout>
    <div slot="header"/>
    <nuxt/>
    <div slot="footer"/>
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
</style>
