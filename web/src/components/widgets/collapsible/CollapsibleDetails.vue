<template>
  <details class="nhsuk-details">
    <summary class="nhsuk-details__summary"
             aria-controls="nhsuk-details__text"
             aria-expanded="false">
      <span class="nhsuk-details__summary-text nhsuk-u-margin-bottom-1">
        <slot name="header" />
      </span>
      <slot name="text" />
    </summary>
    <div id="nhsuk-details__text"
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
  mounted() {
    Details();
  },
};
</script>
