<template>
  <form :action="action" method="post" >
    <input :value="$store.state.session.csrfToken" type="hidden" name="csrfToken">
    <slot/>
  </form>
</template>

<script>
export default {
  props: {
    action: {
      type: String,
      required: true,
    },
  },
};
</script>
