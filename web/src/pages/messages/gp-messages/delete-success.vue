<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink
          id="messagesLink"
          :path="messagesPath"
          button-text="gp_messages.deleteSuccess.back"
          @clickAndPrevent="goToMessagesClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { INDEX, GP_MESSAGES } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
  },
  data() {
    return {
      messagesPath: GP_MESSAGES.path,
    };
  },
  fetch({ store, redirect }) {
    if (isFalsy(store.state.gpMessages.messageDeleted)) {
      redirect(INDEX.path);
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
