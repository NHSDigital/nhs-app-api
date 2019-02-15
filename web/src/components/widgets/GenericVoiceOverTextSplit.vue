<template>
  <span :aria-label="splitText"
        :data-sid="dataSid">
    {{ text }}
  </span>
</template>
<script>
export default {
  name: 'GenericVoiceOverTextSplit',
  props: {
    text: {
      type: String,
      default: '',
    },
    dataSid: {
      type: String,
      default: '',
    },
  },
  computed: {
    splitText() {
      const textNoSpace = this.text.toString()
        .split(' ')
        .join('');

      return textNoSpace.split('').join(', ');
    },
  },
};
</script>
