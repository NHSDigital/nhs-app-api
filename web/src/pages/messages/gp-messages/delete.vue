<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="deleteContentPara1" class="nhsuk-u-font-size-19 nhsuk-u-margin-top-3"
         :aria-label="$t('gp_messages.delete.firstParagraph')">
        {{ $t('gp_messages.delete.firstParagraph') }}
      </p>
      <p id="deleteContentPara2" class="nhsuk-u-font-size-19"
         :aria-label="$t('gp_messages.delete.secondParagraph')">
        {{ $t('gp_messages.delete.secondParagraph') }}
      </p>
      <generic-button id="deleteButton"
                      :class="[$style['nhsuk-button'], 'nhsuk-u-margin-top-2', 'nhsuk-u-padding-2']"
                      @click="deleteButtonClicked">
        {{ $t('gp_messages.delete.deleteButtonText') }}
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
  GP_MESSAGES,
  GP_MESSAGES_VIEW_MESSAGE,
  GP_MESSAGES_DELETE_SUCCESS,
} from '@/lib/routes';
import { redirectTo, isBlankString } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    GenericButton,
  },
  data() {
    return {
      messagesPath: GP_MESSAGES.path,
      messageDetailsPath: GP_MESSAGES_VIEW_MESSAGE.path,
    };
  },
  computed: {
    messageID() {
      return this.$store.state.gpMessages.selectedMessageId;
    },
    buttonText() {
      return 'gp_messages.delete.backButtonText.text';
    },
  },
  fetch({ store, redirect }) {
    if (isBlankString(store.state.gpMessages.selectedMessageId) ||
        store.app.router.currentRoute.path !== GP_MESSAGES_VIEW_MESSAGE.path) {
      redirect(GP_MESSAGES.path);
    }
  },
  methods: {
    async deleteButtonClicked() {
      await this.$store.dispatch('gpMessages/deleteMessage',
        this.$store.state.gpMessages.selectedMessageId);

      if (this.$store.state.gpMessages.messageDeleted) {
        redirectTo(this, GP_MESSAGES_DELETE_SUCCESS.path);
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
  @import '../../../style/colours';

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
