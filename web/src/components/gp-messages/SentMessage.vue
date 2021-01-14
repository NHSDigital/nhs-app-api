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
          <formatted-date-time :id="sentPrefixIdentifier+`MessageSentDateTime`+sentIndex"
                               :class="[$style.messageSentDateTime, 'nhsuk-u-font-size-16']"
                               :date-time="message.sentDateTime" />
        </div>
      </div>
    </li>
  </div>
</template>
<script>
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import LinkifyContent from '@/components/widgets/LinkifyContent';

export default {
  name: 'SentMessage',
  components: { FormattedDateTime, LinkifyContent },
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
    getContent() {
      return this.messageContent || '';
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/sent-message";
</style>
