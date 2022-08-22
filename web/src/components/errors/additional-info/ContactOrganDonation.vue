<template>
  <div class="contact-organ-donation" data-purpose="msg-contactdetails">
    <message-text :is-header="true"
                  :unindent="unindent">
      {{ $t('organDonation.contactOrganDonation.email') }}
      <a href="mailto:NHSApp.Enquiries@nhsbt.nhs.uk" :class="$style['inline-hyperlink']">NHSApp.Enquiries@nhsbt.nhs.uk</a>
      {{ $t('organDonation.contactOrganDonation.forHelp') }}
    </message-text>
  </div>
</template>

<script>
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'ContactOrganDonation',
  components: {
    MessageText,
  },
  props: {
    unindent: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/a-inline";
</style>
