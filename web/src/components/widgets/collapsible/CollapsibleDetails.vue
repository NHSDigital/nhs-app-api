<template>
  <details class="nhsuk-details">
    <summary class="nhsuk-details__summary"
             aria-controls="nhsuk-details__text"
             aria-expanded="false">
      <span class="nhsuk-details__summary-text">
        <slot name="header" />
      </span>
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
