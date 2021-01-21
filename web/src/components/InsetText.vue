<template>
  <div :class="{
    'nhsuk-inset-text': true,
    'nhsuk-u-margin-top-1 nhsuk-u-margin-bottom-1': compact,
  }" data-purpose="inset-text">
    <span class="nhsuk-u-visually-hidden">{{ $t('components.insetText.heading') }}</span>
    <slot/>
  </div>
</template>

<script>

export default {
  name: 'InsetText',
  props: {
    compact: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
};
</script>
