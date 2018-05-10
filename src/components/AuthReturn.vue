<template>
  <div>
    <spinner/>
  </div>
</template>

<script>
import Spinner from '@/components/Spinner';

export default {
  components: {
    Spinner,
  },
  name: '',
  mounted() {
    this.$nextTick(() => {
      this.$store.dispatch('auth/handleAuthResponse', { API_HOST: this.$config.API_HOST, code: this.$route.query.code });
    });
  },
  render() {
    return null;
  },
};
</script>
