<template>
  <div class="nhsuk-inset-text">
    <span class="nhsuk-u-visually-hidden">{{ $t('generic.insetConent.heading') }}</span>
    <div v-for="(paragraph, index) of paragraphs" :key="paragraph">
      <br v-if="index > 0" />
      <p>{{ paragraph }}</p>
    </div>
  </div>
</template>

<script>

export default {
  name: 'InsetContent',
  props: {
    paragraphs: {
      type: Array,
      default: () => [],
      required: true,
    },
  },
};
</script>
