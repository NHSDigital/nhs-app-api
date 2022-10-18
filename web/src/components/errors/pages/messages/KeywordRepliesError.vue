<template>
  <div>
    <div v-if="errorCount === 1">
      <h1 tabindex="-1" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3 break">
        {{ $t("messages.notificationErrorMessages.error.title") }}</h1><br>
      <p>{{ $t("messages.notificationErrorMessages.error.technicalError.message") }}</p>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </div>

    <div v-if="errorCount > 1">
      <h1 tabindex="-1" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3 break">
        {{ $t("messages.notificationErrorMessages.error.title") }}</h1><br>
      <p>{{ $t("messages.notificationErrorMessages.error.tryAgainMessage") }}</p>
      <p>{{ $t("messages.notificationErrorMessages.error.technicalMessageError") }}</p>
      <h2 class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3 break">
        {{ $t("messages.notificationErrorMessages.error.medicalAdviceHeading") }}</h2>
      <p>{{ $t("messages.notificationErrorMessages.error.medicalAdviceParagraph") }}</p>
      <contact-111
        :text="$t('messages.notificationErrorMessages.error.urgentMedicalAdvice')"
        :aria-label="$t('messages.notificationErrorMessages.error.urgentMedicalAdvice')"/>
    </div>

    <desktop-generic-back-link v-if="!isNativeApp" data-purpose="back-link"
                               :path="backLink"
                               @clickAndPrevent="backClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import Contact111 from '@/components/widgets/Contact111';
import { HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import get from 'lodash/fp/get';

export default {
  name: 'KeywordRepliesError',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    Contact111,
  },
  props: {
    errorCount: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      backLink: HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH,
      isNativeApp: this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    message() {
      return this.$store.state.messaging.message;
    },
    sender() {
      return get('senderId')(this.message);
    },
  },
  created() {
    this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
  },
  methods: {
    backClicked() {
      redirectTo(this, this.backLink, { senderId: this.sender });
    },
    tryAgain() {
      this.$store.dispatch('messaging/clearErrorReply');
    },
  },
};
</script>
