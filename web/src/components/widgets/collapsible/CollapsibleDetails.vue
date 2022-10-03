<template>
  <details class="nhsuk-details">
    <summary class="nhsuk-details__summary"
             :aria-controls="id"
             aria-expanded="false">
      <span class="nhsuk-details__summary-text nhsuk-u-margin-bottom-1">
        <slot name="header" />
      </span>
      <slot name="text" />
    </summary>
    <div :id="id"
         class="nhsuk-details__text"
         aria-hidden="true">
      <slot/>
    </div>
  </details>
</template>

<script>
import Details from './details';

export default {
  name: 'CollapsibleDetails',
  props: {
    id: {
      type: String,
      required: false,
      default: 'nhsuk-details__text',
    },
  },
  mounted() {
    Details();
  },
};
</script>
