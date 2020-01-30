<script>
export default {
  watch: {
    error(value) {
      if (value) {
        window.scrollTo(0, 0);
      }
    },
  },
};
</script>
