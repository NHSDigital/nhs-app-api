<template>
  <div>
    <div :class="$style['nhsuk-panel-sender']">
      <span :id="idPrefix+`MessageReplySender`+index"
            class="nhsuk-u-font-size-16">{{ message.sender }}</span>
    </div>
    <div :id="idPrefix+`MessageReplyPanel`+index"
         :class="[$style['nhsuk-panel'],
                  'nhsuk-u-margin-top-0',
                  'nhsuk-u-margin-bottom-0',
                  'nhsuk-u-padding-2']">
      <linkify-content class="panel-content nhsuk-u-font-size-19"
                       :content="message.replyContent" tag="p"/>
    </div>
    <p :id="idPrefix+`MessageReplyDateTime`+index"
       class="nhsuk-u-font-size-16 nhsuk-u-margin-bottom-0">
      {{ formattedTime }}
    </p>
  </div>
</template>
<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';
import { formatIndividualMessageTime } from '@/lib/utils';

export default {
  name: 'ReceivedMessagePanel',
  components: { LinkifyContent },
  props: {
    index: {
      type: Number,
      required: true,
    },
    idPrefix: {
      type: String,
      required: true,
    },
    message: {
      type: Object,
      required: true,
    },
  },
  computed: {
    formattedTime() {
      return formatIndividualMessageTime(this.message.sentDateTime, this.$t.bind(this));
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

  .nhsuk-panel {
    border: 1px solid $border_grey;
    width: fit-content;
  }

  .nhsuk-panel-sender {
    text-align: left;
  }
</style>
