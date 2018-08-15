<template>
  <div v-if="showTemplate" class="pull-content">
    <h2 data-purpose="greeting">{{ greetingMessage }}</h2>
    <p data-purpose="info">{{ $t('homeLoggedIn.description') }}</p>
  </div>
</template>

<script>

export default {
  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { name } = this.$store.state.auth.user;
      return `${message}, ${name}`;
    },
  },
};
</script>
