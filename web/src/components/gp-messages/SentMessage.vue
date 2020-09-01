<template>
  <div>
    <li :id="sentPrefixIdentifier+`SentMessage`+sentIndex"
        :class="$style['nhsuk-panel-group__item']">
      <div :class="$style['nhsuk-panel-sender-container']">
        <div :class="$style['nhsuk-panel-sender']">
          <span class="nhsuk-u-font-size-16">
            {{ $t('messages.you') }}
          </span>
        </div>
        <div :id="sentPrefixIdentifier+`MessageSentPanel`+sentIndex"
             :class="[$style['nhsuk-panel'], 'nhsuk-u-margin-top-0', 'nhsuk-u-padding-2']">
          <h2 v-if="hasSubject" :id="sentPrefixIdentifier+`MessageSubject`+sentIndex"
              class="nhsuk-heading-s nhsuk-u-font-size-19 nhsuk-u-margin-bottom-0">
            {{ message.subject }}</h2>
          <linkify-content class="panel-content nhsuk-u-font-size-19"
                           :content="getContent" tag="p"/>
        </div>
        <div>
          <p :id="sentPrefixIdentifier+`MessageSentDateTime`+sentIndex"
             :class="[$style.messageSentDateTime,
                      'nhsuk-u-font-size-16',
                      'nhsuk-u-margin-bottom-0']">
            {{ formattedTime }}
          </p>
        </div>
      </div>
    </li>
  </div>
</template>
<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';
import { formatIndividualMessageTime } from '@/lib/utils';

export default {
  name: 'SentMessage',
  components: { LinkifyContent },
  props: {
    message: {
      type: Object,
      required: true,
    },
    sentIndex: {
      type: Number,
      required: true,
    },
    sentPrefixIdentifier: {
      type: String,
      required: true,
    },
    messageContent: {
      type: String,
      default: '',
    },
  },
  computed: {
    getSentMessage() {
      if (this.$store.state.gpMessages
        .selectedMessageDetails.messageDetails.messageFromPatient === false) {
        return undefined;
      }
      return this.$store.state.gpMessages
        .selectedMessageDetails.messageDetails;
    },
    hasSubject() {
      return this.message.subject !== undefined;
    },
    formattedTime() {
      return formatIndividualMessageTime(this.message.sentDateTime, this.$t.bind(this));
    },
    getContent() {
      return this.messageContent || '';
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

  .nhsuk-panel-sender {
    text-align: right;
  }

  .messageSentDateTime {
    text-align: right;
    float: right;
  }
</style>
