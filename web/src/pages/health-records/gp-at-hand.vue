<template>
  <div v-if="showTemplate">

    <gp-at-hand-content
      :header-tag="'gpAtHand.myRecord.headerTag'"
      :content-tag="'gpAtHand.myRecord.contentTag'" />

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
