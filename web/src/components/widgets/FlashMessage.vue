<template>
  <div v-if="showFlashMessage">
    <message-dialog-generic v-if="isWarning()" :extra-classes="[$style['flash-message']]"
                            message-type="warning" >
      <message-text>{{ message }}</message-text>
    </message-dialog-generic>
    <message-dialog v-else-if="isError()" :extra-classes="[$style['flash-message']]"
                    message-type="error" >
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <message-dialog-generic v-else :extra-classes="[$style['flash-message']]"
                            message-id="success-dialog"
                            message-type="success" >
      <message-text>{{ message }}</message-text>
    </message-dialog-generic>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'FlashMessage',
  components: {
    MessageDialog,
    MessageDialogGeneric,
    MessageText,
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
        if (this.$store.state.flashMessage.key) {
          return this.$t(this.$store.state.flashMessage.key);
        }
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
    isError() {
      return this.$store.state.flashMessage.type === 'error';
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/flash-message";
</style>
