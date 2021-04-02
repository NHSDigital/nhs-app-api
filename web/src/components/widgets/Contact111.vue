<template>
  <p :class="clazz" :aria-label="ariaLabel" data-purpose="msg-text">
    {{ text }}
    <a :href="symptomCheckerUrl" target="_blank" rel="noopener noreferrer" style="display:inline">
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
  data() {
    return {
      symptomCheckerUrl: this.$store.$env.SYMPTOM_CHECKER_URL,
    };
  },
};
</script>
