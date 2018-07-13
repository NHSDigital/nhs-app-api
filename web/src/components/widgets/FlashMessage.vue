<template>
  <div v-if="showFlashMessage" :class="$style.flashMessage">
    <error-warning-dialog v-if="isWarning()" error-or-warning = "warning" >
      <p>
        {{ message }}
      </p>
    </error-warning-dialog>
    <success-dialog v-else >
      <p>
        {{ message }}
      </p>
    </success-dialog>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import SuccessDialog from '@/components/widgets/SuccessDialog';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  components: {
    SuccessDialog,
    ErrorWarningDialog,
  },
  computed: {
    showFlashMessage() {
      if (this.showMessage()) {
        this.$store.dispatch('flashMessage/hasBeenShown');
        return true;
      }

      return false;
    },
    message() {
      if (this.showMessage()) {
        return this.$store.state.flashMessage.message;
      }

      return '';
    },
  },
  methods: {
    showMessage() {
      return this.$store.state.flashMessage.show;
    },
    isWarning() {
      return this.$store.state.flashMessage.type === 'warning';
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";
  @import "../../style/textstyles";
  @import "../../style/colours";

  .flashMessage {
    @include space(padding, all, $three);
  }

</style>
