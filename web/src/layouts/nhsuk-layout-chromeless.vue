<template>
  <nhs-uk-layout :has-footer="false">
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
