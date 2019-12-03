<template>
  <ul :class="[$style['nhsuk-app-chat'],
               'nhsuk-u-margin-top-2',
               'nhsuk-u-margin-bottom-4',
               'nhsuk-u-padding-left-0']">
    <li v-for="(reply, index) in getReplies" :id="`messageReply`+index"
        :key="`messageReply`+index"
        :class="[$style['nhsuk-panel-group__item'], 'nhsuk-u-padding-bottom-3']">
      <div>
        <div :class="$style['nhsuk-panel-sender']">
          <span :id="`messageReplySender`+index"
                class="nhsuk-u-font-size-16">{{ reply.sender }}</span>
        </div>
        <div :id="`messageReplyPanel`+index"
             :class="[$style['nhsuk-panel'], 'nhsuk-u-margin-top-0', 'nhsuk-u-padding-2']">
          <linkify-content class="panel-content nhsuk-u-font-size-19"
                           :content="reply.replyContent" tag="p"/>
        </div>
        <time :id="`messageReplyDateTime`+index"
              class="nhsuk-u-font-size-16"
              :datetime="reply.sentDateTime | formatDate('YYYY-MM-DD h:mma')">
          Sent {{ reply.sentDateTime | formatDate('DD MMMM YYYY') }}
          at {{ reply.sentDateTime | formatDate('h:mma') }}
        </time>
      </div>
    </li>
  </ul>
</template>
<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';

export default {
  name: 'ReceivedMessage',
  components: { LinkifyContent },
  computed: {
    getReplies() {
      return this.$store.state.patientPracticeMessaging
        .selectedMessageDetails.messageDetails.messageReplies;
    },
  },
};
</script>
<style lang="scss">

</style>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/components/panel/panel';
  @import "../../style/colours";

  .nhsuk-panel-group__item {
    padding-right: 15%;

    .nhsuk-panel {
      border: 1px solid $border_grey;
      width: fit-content;
    }
  }
  .nhsuk-app-chat {
    list-style-type: none;

    .nhsuk-panel-sender {
      text-align: left;
    }
  }

</style>
