<template>
  <card>
    <p>
      {{ $t('myRecord.noFurtherInformationAvailable') }}
    </p>
  </card>
</template>

<script>
import Card from '@/components/widgets/card/Card';

export default {
  name: 'NoFurtherInformationAvailable',
  components: {
    Card,
  },
};
</script>
