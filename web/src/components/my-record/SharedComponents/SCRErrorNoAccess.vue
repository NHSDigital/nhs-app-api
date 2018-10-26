<template>
  <div :class="[$style['record-content']]">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
</template>

<script>


export default {
  props: {
    data: {
      type: Object,
      default: () => {},
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
