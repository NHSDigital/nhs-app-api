<template>
  <ul :v-if="getSentMessage !== undefined"
      :class="[$style['nhsuk-app-chat'], 'nhsuk-u-margin-bottom-0', 'nhsuk-u-padding-left-0']">
    <li id="sentMessage" :class="$style['nhsuk-panel-group__item']">
      <div :class="$style['nhsuk-panel-sender-container']">
        <div>
          <div :class="$style['nhsuk-panel-sender']">
            <span class="nhsuk-u-font-size-16">You</span>
          </div>
          <div id="messageSentPanel"
               :class="[$style['nhsuk-panel'], 'nhsuk-u-margin-top-0', 'nhsuk-u-padding-2']">
            <h2 id="messageSubject"
                class="nhsuk-heading-s nhsuk-u-font-size-19 nhsuk-u-margin-bottom-0">
              {{ getSentMessage.subject }}</h2>
            <linkify-content
              class="panel-content nhsuk-u-font-size-19"
              :content="getSentMessage.content" tag="p"/>
          </div>
        </div>
        <div>
          <time id="messageSentDateTime"
                class="nhsuk-u-font-size-16"
                :datetime="getSentMessage.sentDateTime | formatDate('YYYY-MM-DD h:mma')">
            Sent {{ getSentMessage.sentDateTime | formatDate('DD MMMM YYYY') }}
            at {{ getSentMessage.sentDateTime | formatDate('h:mma') }}
          </time>
        </div>
      </div>
    </li>
  </ul>
</template>
<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';

export default {
  name: 'SentMessage',
  components: { LinkifyContent },
  computed: {
    getSentMessage() {
      return this.$store.state.patientPracticeMessaging
        .selectedMessageDetails.messageDetails;
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

  time {
    text-align: right;
    float: right;
  }

</style>
