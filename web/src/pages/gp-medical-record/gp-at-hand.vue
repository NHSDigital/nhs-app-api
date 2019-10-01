<template>
  <div v-if="showTemplate">

    <gp-at-hand-content
      :header-tag="'gp_at_hand.myRecord.headerTag'"
      :content-tag="'gp_at_hand.myRecord.contentTag'" />

  </div>
</template>

<script>
import GpAtHandContent from '@/components/GpAtHandContent';

export default {
  components: {
    GpAtHandContent,
  },
};
</script>

<style module lang="scss" scoped>

</style>
