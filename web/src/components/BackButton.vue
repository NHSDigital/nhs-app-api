<template>
  <generic-button :class="[$style.button, $style.grey]"
                  @click.stop.prevent="goBack">
    {{ buttonText }}
  </generic-button>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';

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
      if (this.gotoPath) redirectTo(this, this.gotoPath, null);
      else this.$router.goBack();
    },
  },
};
</script>
<style module lang="scss">
  @import "../style/buttons";
</style>
