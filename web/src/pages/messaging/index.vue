<template>
  <div v-if="showTemplate" id="mainDiv"/>
</template>

<script>
import { MESSAGING_MESSAGES } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  created() {
    redirectTo(this, MESSAGING_MESSAGES.path, null);
  },
};
</script>
