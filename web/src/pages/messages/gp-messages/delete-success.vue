<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <desktopGenericBackLink
        id="messagesLink"
        :path="messagesPath"
        button-text="gp_messages.deleteSuccess.back"
        @clickAndPrevent="goToMessagesClicked"/>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { INDEX_PATH, GP_MESSAGES_PATH } from '@/router/paths';
import { isFalsy, redirectTo } from '@/lib/utils';

export default {
  name: 'GpMessageDeleteSuccessPage',
  components: {
    DesktopGenericBackLink,
  },
  data() {
    return {
      recipient: get('$store.state.gpMessages.selectedMessageRecipient.name')(this),
      messagesPath: GP_MESSAGES_PATH,
    };
  },
  created() {
    if (isFalsy(this.$store.state.gpMessages.messageDeleted)) {
      redirectTo(this, INDEX_PATH);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('gpMessages/clear');
  },
  methods: {
    goToMessagesClicked() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>
