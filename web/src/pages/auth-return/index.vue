
<script>
export default {
  name: '',
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
