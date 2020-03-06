<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="deleteContentPara1" class="nhsuk-u-font-size-19 nhsuk-u-margin-top-3"
         :aria-label="$t('patient_practice_messaging.delete.firstParagraph')">
        {{ $t('patient_practice_messaging.delete.firstParagraph') }}
      </p>
      <p id="deleteContentPara2" class="nhsuk-u-font-size-19"
         :aria-label="$t('patient_practice_messaging.delete.secondParagraph')">
        {{ $t('patient_practice_messaging.delete.secondParagraph') }}
      </p>
      <generic-button id="deleteButton"
                      :class="[$style['nhsuk-button'], 'nhsuk-u-margin-top-2', 'nhsuk-u-padding-2']"
                      @click="deleteButtonClicked">
        {{ $t('patient_practice_messaging.delete.deleteButtonText') }}
      </generic-button>
      <desktop-generic-back-link :path="messageDetailsPath"
                                 :button-text="buttonText"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import {
  PATIENT_PRACTICE_MESSAGING,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
  PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS,
} from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    GenericButton,
  },
  data() {
    return {
      messagesPath: PATIENT_PRACTICE_MESSAGING.path,
      messageDetailsPath: PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.path,
    };
  },
  computed: {
    messageID() {
      return this.$store.state.patientPracticeMessaging.selectedMessageId;
    },
    buttonText() {
      return 'patient_practice_messaging.delete.backButtonText.text';
    },
  },
  fetch({ store, redirect }) {
    if (store.state.patientPracticeMessaging.selectedMessageId === undefined ||
        store.app.router.currentRoute.path !== PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.path) {
      redirect(PATIENT_PRACTICE_MESSAGING.path);
    }
  },
  methods: {
    async deleteButtonClicked() {
      await this.$store.dispatch('patientPracticeMessaging/deleteMessage',
        this.$store.state.patientPracticeMessaging.selectedMessageId);
      if (this.$store.state.patientPracticeMessaging.messageDeleted) {
        redirectTo(this, PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS.path);
      }
    },
    backLinkClicked() {
      redirectTo(this, this.messageDetailsPath);
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/components/button/button';
  @import '../../style/colours';

  .nhsuk-button{
    background-color: $button_red;

    &:focus {
      background-color: $button_red_focus;
    }

    &:hover {
      background-color: $button_red_focus;
    }
  }
  </style>
