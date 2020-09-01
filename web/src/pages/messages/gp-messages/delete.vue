<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="deleteContentPara1" class="nhsuk-u-font-size-19 nhsuk-u-margin-top-3"
         :aria-label="$t('messages.deletingYourConversationWillRemoveIt')">
        {{ $t('messages.deletingYourConversationWillRemoveIt') }}
      </p>
      <p id="deleteContentPara2" class="nhsuk-u-font-size-19"
         :aria-label="$t('messages.yourConversationWillStillBeSaved')">
        {{ $t('messages.yourConversationWillStillBeSaved') }}
      </p>
      <generic-button id="deleteButton"
                      :class="[$style['nhsuk-button'],
                               'nhsuk-u-margin-top-2',
                               'nhsuk-u-padding-2']"
                      @click="deleteButtonClicked">
        {{ $t('messages.deleteConversation') }}
      </generic-button>
      <desktop-generic-back-link :path="messageDetailsPath"
                                 :button-text="buttonText"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo, isBlankString } from '@/lib/utils';
import {
  GP_MESSAGES_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_DELETE_SUCCESS_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesDeletePage',
  components: {
    DesktopGenericBackLink,
    GenericButton,
  },
  data() {
    return {
      recipient: get('$store.state.gpMessages.selectedMessageRecipient.name')(this),
      messagesPath: GP_MESSAGES_PATH,
      messageDetailsPath: GP_MESSAGES_VIEW_MESSAGE_PATH,
    };
  },
  computed: {
    messageID() {
      return this.$store.state.gpMessages.selectedMessageId;
    },
    buttonText() {
      return 'messages.cancel';
    },
  },
  beforeRouteEnter(_, from, next) {
    next(from.path !== GP_MESSAGES_VIEW_MESSAGE_PATH ? GP_MESSAGES_PATH : undefined);
  },
  created() {
    if (isBlankString(this.$store.state.gpMessages.selectedMessageId)) {
      redirectTo(this, this.messagesPath);
    }
  },
  methods: {
    async deleteButtonClicked() {
      await this.$store.dispatch('gpMessages/deleteMessage',
        this.$store.state.gpMessages.selectedMessageId);

      if (this.$store.state.gpMessages.messageDeleted) {
        redirectTo(this, GP_MESSAGES_DELETE_SUCCESS_PATH);
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
