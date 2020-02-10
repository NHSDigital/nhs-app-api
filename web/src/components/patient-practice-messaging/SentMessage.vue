<template>
  <ul v-if="getSentMessage !== undefined"
      :class="[$style['nhsuk-app-chat'], 'nhsuk-u-margin-bottom-0', 'nhsuk-u-padding-left-0']">
    <li id="sentMessage" :class="$style['nhsuk-panel-group__item']">
      <div :class="$style['nhsuk-panel-sender-container']">
        <div :class="$style['nhsuk-panel-sender']">
          <span class="nhsuk-u-font-size-16">
            {{ $t('patient_practice_messaging.view_details.you') }}
          </span>
        </div>
        <div id="messageSentPanel"
             :class="[$style['nhsuk-panel'], 'nhsuk-u-margin-top-0', 'nhsuk-u-padding-2']">
          <h2 id="messageSubject"
              class="nhsuk-heading-s nhsuk-u-font-size-19 nhsuk-u-margin-bottom-0">
            {{ getSentMessage.subject }}</h2>
          <linkify-content class="panel-content nhsuk-u-font-size-19"
                           :content="getSentMessage.content" tag="p"/>
        </div>
        <div>
          <p id="messageSentDateTime"
             :class="[$style.messageSentDateTime,
                      'nhsuk-u-font-size-16',
                      'nhsuk-u-margin-bottom-0']">
            {{ formattedTime }}
          </p>
        </div>
      </div>
    </li>
  </ul>
</template>
<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';
import { formatIndividualMessageTime } from '@/lib/utils';

export default {
  name: 'SentMessage',
  components: { LinkifyContent },
  computed: {
    getSentMessage() {
      return this.$store.state.patientPracticeMessaging
        .selectedMessageDetails.messageDetails;
    },
    formattedTime() {
      return formatIndividualMessageTime(this.getSentMessage.sentDateTime, this.$t.bind(this));
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
  @import '~nhsuk-frontend/packages/components/panel/panel';
  @import "../../style/colours";

  .nhsuk-panel-group__item {
    padding-left: 15%;
    text-align: right;
    width: 100%;
    display: inline-block;

    .nhsuk-panel-sender-container{
      float: right;
    }

    .nhsuk-panel {
      background-color: $grey_panel;
      border: 1px solid $border_grey;
      text-align: left;
      width: fit-content;
      float: right;
    }
  }

  .nhsuk-app-chat {
    list-style-type: none;

    .nhsuk-panel-sender {
      text-align: right;
    }
  }

  .messageSentDateTime {
    text-align: right;
    float: right;
  }
</style>
