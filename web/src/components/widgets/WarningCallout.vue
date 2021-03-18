<template>
  <div class="nhsuk-warning-callout" data-purpose="warning-callout">
    <h3 class="nhsuk-warning-callout__label"
        data-purpose="warning-callout-title"
        :aria-label="headingAriaLabel">{{ title }}</h3>
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
  computed: {
    headingAriaLabel() {
      return this.title.toLowerCase().indexOf('important') < 0 ? `Important: ${this.title}` : undefined;
    },
  },
};
</script>
