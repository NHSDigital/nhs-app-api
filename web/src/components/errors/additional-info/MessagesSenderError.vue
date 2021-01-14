<template>
  <message-text>
    {{ errorText }}
  </message-text>
</template>

<script>
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'MessagesSenderError',
  components: {
    MessageText,
  },
  computed: {
    errorText() {
      return this.$t('messages.ifTheProblemContinuesContantSenderNameDirectly').replace('{senderName}', this.$store.state.messaging.selectedSender);
    },
  },
};
</script>
