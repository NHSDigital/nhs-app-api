<template>
  <div class="contact-organ-donation" data-purpose="msg-contactdetails">
    <message-text :is-header="true"
                  :unindent="unindent">
      {{ $t('organDonation.contactOrganDonation.email') }}
    </message-text>
    <message-text :unindent="unindent">
      <a href="mailto:NHSApp.Enquiries@nhsbt.nhs.uk">NHSApp.Enquiries@nhsbt.nhs.uk</a>
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
