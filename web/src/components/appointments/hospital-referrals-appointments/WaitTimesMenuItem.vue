<template>
  <div v-if="isWaitTimesEnabled">
    <h2 id="wait-times-header">
      Hi! I'm a Wait Times section. Please wait...
    </h2>
  </div>
</template>

<script>

export default {
  name: 'WaitTimesMenuItem',

  props: {
    isWaitTimesEnabled: {
      type: Boolean,
      default: () => false,
    },
  },
};
</script>
