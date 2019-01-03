<template>
  <div v-if="showFlashMessage" :class="[isDesktopWeb ? $style.flashMessage : undefined]">
    <message-dialog v-if="isWarning()" message-type="warning" >
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <message-dialog v-else message-id="success-dialog"
                    message-type="success">
      <message-text :class="[isDesktopWeb ? $style.messageText : undefined]">{{ message }}
      </message-text>
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
  data() {
    return {
      isDesktopWeb: (this.$store.state.device.source !== 'android' && this.$store.state.device.source !== 'ios'),
    };
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
 @import '../../style/textstyles';
 @import "../../style/fonts";

 .flashMessage{
  font-family: $default-web;
  max-width: 540px;
 }

 .messageText{
  font-family: $default-web;
  font-weight: lighter;
 }

</style>
