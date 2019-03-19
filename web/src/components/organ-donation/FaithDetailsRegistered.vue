<template>
  <div id="faithAndBeliefs" :class="$style.info" >
    <p>{{ $t(`organDonation.registered.faithAndBeliefs.${declaration}`) }}</p>
  </div>
</template>
<script>
export default {
  name: 'FaithDetailsRegistered',
  props: {
    declaration: {
      type: String,
      required: true,
    },
  },
};
</script>
<style module lang="scss">
@import "../../style/info";
</style>
