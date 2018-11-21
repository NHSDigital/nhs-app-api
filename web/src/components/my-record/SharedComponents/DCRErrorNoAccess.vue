<template>
  <div :class="[$style['record-content']]">
    <p v-if="hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else-if="!hasAccess">
      {{ $t('my_record.genericNoAccessMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
</template>

<script>


export default {
  props: {
    hasAccess: {
      type: Boolean,
      default: () => false,
    },
    hasErrored: {
      type: Boolean,
      default: () => false,
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
