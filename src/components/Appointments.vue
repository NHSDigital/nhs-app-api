<template>
  <div id="mainDiv">
    <spinner></spinner>
    <main class="content">
    </main>
  </div>
</template>

<script>
import Spinner from '@/components/Spinner';

export default {
  components: {
    Spinner,
  },
  mounted() {
    this.$root.$emit('show-loading-spinner');
  },
};

</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';
</style>
