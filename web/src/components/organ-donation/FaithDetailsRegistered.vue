<template>
  <p id="faithAndBeliefs" >
    {{ $t(`organDonation.faith.registered.${declaration}`) }}
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
