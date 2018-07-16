
<script>
export default {
  name: '',
  middleware: ['authResponse'],
  fetch({ redirect }) {
    if (process.server) {
      redirect('/');
    }
  },
  render() {
    return null;
  },
};
</script>
