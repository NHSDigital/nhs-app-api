<template>
  <collapsible-details :id="id" class="nhsuk-expander">
    <template slot="header">
      <slot name="header" />
    </template>
    <slot/>
  </collapsible-details>
</template>

<script>
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';

export default {
  name: 'CollapsibleDialog',
  components: {
    CollapsibleDetails,
  },
  props: {
    id: {
      type: String,
      required: false,
      default: 'nhsuk-details__text',
    },
  },
};
</script>

