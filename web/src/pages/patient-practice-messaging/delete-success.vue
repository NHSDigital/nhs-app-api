<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink
          id="messagesLink"
          :path="messagesPath"
          button-text="patient_practice_messaging.deleteSuccess.back"
          @clickAndPrevent="goToMessagesClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { INDEX, PATIENT_PRACTICE_MESSAGING } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
  },
  data() {
    return {
      messagesPath: PATIENT_PRACTICE_MESSAGING.path,
    };
  },
  fetch({ store, redirect }) {
    if (isFalsy(store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED) ||
    isFalsy(store.state.patientPracticeMessaging.messageDeleted)) {
      redirect(INDEX.path);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('patientPracticeMessaging/clear');
  },
  methods: {
    goToMessagesClicked() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>
