<template>
  <div class="bodyDiv">
    <h1>{{ title }}</h1>
    <button id="get-values-button" v-on:click="getValues">{{ $t('message.getValue') }}</button>
    <ul id="values-list">
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
      title: 'NHS Online',
      values: [],
    };
  },
  methods: {
    getValues() {
      return this.nhsOnlineApi.getValues().then((values) => {
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
