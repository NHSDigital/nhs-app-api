<template>
  <div class="bodyDiv">
    <h1>{{ msg }}</h1>
    <button v-on:click="getValues">Get Values</button>
    <ul>
      <li v-for="value in values" :key="value.name">
        {{ value.name }}
      </li>
    </ul>
  </div>
</template>


<script>

export default {
  name: 'HelloWorld',
  inject: [
    'nhsOnlineApi',
  ],
  data() {
    return {
      msg: 'NHS Online',
      values: [],
    };
  },
  methods: {
    getValues() {
      this.nhsOnlineApi.getValues()
        .then((values) => {
          this.values = values;
        });
    },
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
