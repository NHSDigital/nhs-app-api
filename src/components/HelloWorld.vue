<template>
  <div class="bodyDiv">
    <h1>{{ msg }}</h1>
  </div>
</template>

<script>
export default {
  name: 'HelloWorld',
  data() {
    return {
      msg: 'NHS Online',
    };
  },
};
</script>

<style lang="scss" scoped>
@import "../style/nhso.scss";

h1 {
  @include h1;
  background-color: $nhs_blue;
}

</style>
