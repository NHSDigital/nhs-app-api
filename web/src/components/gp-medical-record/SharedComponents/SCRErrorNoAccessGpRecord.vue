<template>
  <card class="nhsuk-u-margin-bottom-6">
    <p v-if="hasErrored" class="nhsuk-u-margin-0">
      {{ $t('myRecord.anErrorHasOccurredRetrievingData') }}
    </p>
    <p v-else-if="hasUndeterminedAccess" class="nhsuk-u-margin-0">
      {{ $t('myRecord.thisInfoIsNotAvailabeInTheApp') }}
    </p>
    <p v-else class="nhsuk-u-margin-0">
      {{ $t('myRecord.noInformationAvailable') }}
    </p>
  </card>
</template>

<script>
import Card from '@/components/widgets/card/Card';

export default {
  name: 'SCRErrorNoAccessGpRecord',
  components: {
    Card,
  },
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
