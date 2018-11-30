<template>
  <div v-if="showFlashMessage" class="pull-content">
    <message-dialog v-if="isWarning()" message-type="warning" >
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <message-dialog v-else message-id="success-dialog"
                    message-type="success">
      <message-text>{{ message }}</message-text>
    </message-dialog>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
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
  },
};
</script>

<style module lang="scss" scoped>

</style>
