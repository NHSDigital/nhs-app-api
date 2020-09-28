<template>
  <p :class="clazz" :aria-label="ariaLabel" data-purpose="msg-text">
    {{ text }}
    <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer" style="display:inline">
      {{ $t('generic.nhs111Link') }}</a>
    {{ $t('generic.orCall') }}
  </p>
</template>

<script>
export default {
  name: 'Contact111',
  props: {
    text: {
      type: String,
      required: true,
    },
    ariaLabel: {
      type: String,
      default: undefined,
    },
    clazz: {
      type: String,
      default: undefined,
    },
  },
};
</script>
