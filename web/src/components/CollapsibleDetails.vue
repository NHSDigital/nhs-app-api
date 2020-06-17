<template>
  <details class="nhsuk-details">
    <summary class="nhsuk-details__summary">
      <span class="nhsuk-details__summary-text">
        <slot name="header" />
      </span>
    </summary>
    <div class="nhsuk-details__text">
      <slot/>
    </div>
  </details>
</template>

<script>

export default {
  name: 'CollapsibleDetails',
};
</script>

