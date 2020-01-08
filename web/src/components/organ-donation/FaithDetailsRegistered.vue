<template>
  <p id="faithAndBeliefs" >
    {{ $t(`organDonation.registered.faithAndBeliefs.${declaration}`) }}
  </p>
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
