<template>
  <div v-if="showFlashMessage">
    <message-dialog v-if="isWarning()" :extra-classes="[$style['flash-message']]"
                    message-type="warning" >
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <message-dialog v-else-if="isError()" :extra-classes="[$style['flash-message']]"
                    message-type="error" >
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <message-dialog v-else :extra-classes="[$style['flash-message']]"
                    message-id="success-dialog"
                    message-type="success">
      <message-text>{{ message }}
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'FlashMessage',
  components: {
    MessageDialog,
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
.flash-message {
  margin-top: 1.125em;
}
</style>
