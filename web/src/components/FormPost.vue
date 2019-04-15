<template>
  <form :action="action" method="post" @submit.prevent="() => {}" >
    <input :value="$store.state.session.csrfToken" type="hidden" name="csrfToken">
    <slot/>
  </form>
</template>

<script>
export default {
  name: 'FormPost',
  props: {
    action: {
      type: String,
      required: true,
    },
  },
};
</script>
