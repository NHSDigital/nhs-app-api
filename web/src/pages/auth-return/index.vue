
<script>
export default {
  name: '',
  middleware: ['authResponse'],
  fetch({ redirect }) {
    redirect('/');
  },
  render() {
    return null;
  },
};
</script>
