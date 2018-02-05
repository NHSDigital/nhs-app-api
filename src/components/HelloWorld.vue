<template>
  <div class="bodyDiv">
    <h1>{{ msg }}</h1>
    <h2>Essential Links</h2>
    <p class="testClass">This is a test class</p>

    <h2>Ecosystem</h2>
  </div>
</template>

<script>
export default {
  name: 'HelloWorld',
  data() {
    return {
      msg: 'Welcome to Your Vue.js App',
    };
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import "../style/nhso.scss";
$primary-color:#333;

.testClass{
  background-color: $primary-color;
}

h1, h2 {
  @include screen_title;
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
