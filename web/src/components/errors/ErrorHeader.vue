<template>
  <h2>{{ $t(from) }}</h2>
</template>

<script>

export default {
  name: 'ErrorHeader',
  props: {
    from: {
      type: String,
      required: true,
    },
  },
};
</script>
