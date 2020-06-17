<template>
  <collapsible-details class="nhsuk-expander">
    <template slot="header">
      <slot name="header" />
    </template>
    <slot/>
  </collapsible-details>
</template>

<script>
import CollapsibleDetails from '@/components/CollapsibleDetails';

export default {
  name: 'CollapsibleDialog',
  components: {
    CollapsibleDetails,
  },
};
</script>

