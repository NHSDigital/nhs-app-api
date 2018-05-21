<template>
  <div>
    <spinner/>
  </div>
</template>


<script>
/* eslint-disable import/extensions */
import Spinner from '../../components/Spinner';

export default {
  name: '',
  components: {
    Spinner,
  },
  mounted() {
    this.$nextTick(() => {
      this.$store.dispatch('auth/handleAuthResponse', {
        API_HOST: process.env.API_HOST,
        code: this.$route.query.code,
      });
    });
  },
  render() {
    return null;
  },
};
</script>
