<template>
  <generic-button class="nhsuk-button" @click="onClick">
    <slot/>
  </generic-button>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'PrimaryButton',
  components: {
    GenericButton,
  },
  methods: {
    onClick(event) {
      this.$emit('click', event);
    },
  },
};
</script>
