
<script>
export default {
  name: '',
  mounted() {
    return this.$store.dispatch('auth/handleAuthResponse', {
      code: this.$route.query.code,
    });
  },
  render() {
    return null;
  },
};
</script>
