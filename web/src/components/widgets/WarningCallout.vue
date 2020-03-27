<template>
  <div class="nhsuk-warning-callout" data-purpose="warning-callout">
    <h3 class="nhsuk-warning-callout__label" data-purpose="warning-callout-title">{{ title }}</h3>
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'WarningCallout',
  props: {
    title: {
      type: String,
      required: true,
    },
  },
};
</script>
