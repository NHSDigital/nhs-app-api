<template>
  <div :class="[$style['record-content']]">
    <p v-if="hasErrored"
       class="nhsuk-u-padding-3 nhsuk-u-margin-0">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else-if="hasUndeterminedAccess"
       class="nhsuk-u-padding-3 nhsuk-u-margin-0">
      {{ $t('my_record.noRecordsOrNoAccess.warningHeader') }}
    </p>
    <p v-else
       class="nhsuk-u-padding-3 nhsuk-u-margin-0">
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
</template>

<script>


export default {
  name: 'SCRErrorNoAccess',
  props: {
    hasErrored: {
      type: Boolean,
      default: () => false,
    },
    hasUndeterminedAccess: {
      type: Boolean,
      default: () => false,
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
