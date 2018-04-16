<script>

export default {
  name: '',
  mounted() {
    this.$nextTick(() => {
      this.$store.dispatch('auth/handleAuthResponse');
    });
  },
};
</script>
