<template>
  <div class="nhsuk-inset-text">
    <span class="nhsuk-u-visually-hidden">{{ $t('components.insetText.heading') }}</span>
    <p v-for="paragraph of paragraphs" :key="paragraph">
      {{ paragraph }}
    </p>
  </div>
</template>

<script>

export default {
  name: 'InsetText',
  props: {
    paragraphs: {
      type: Array,
      required: true,
    },
  },
};
</script>
