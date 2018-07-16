<script>
export default {
  name: '',
  fetch(context) {
    debugger;
    context.store.dispatch('auth/logout', '/login');
  },
  render() {
    return null;
  },
};
</script>
