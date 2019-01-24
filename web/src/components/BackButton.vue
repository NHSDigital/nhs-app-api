<template>
  <generic-button :class="[$style.button, $style.grey]"
                  @click.stop.prevent="goBack">
    {{ buttonText }}
  </generic-button>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'BackButton',
  components: {
    GenericButton,
  },
  props: {
    gotoPath: {
      type: String,
      default: '',
    },
    text: {
      type: String,
      default: '',
    },
  },
  computed: {
    buttonText() {
      return this.text || this.$t('generic.backButton.text');
    },
  },
  methods: {
    goBack() {
      if (this.gotoPath) this.$router.push(this.gotoPath);
      else this.$router.go(-1);
    },
  },
};
</script>
<style module lang="scss">
  @import "../style/buttons";
</style>
