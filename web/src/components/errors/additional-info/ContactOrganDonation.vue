<template>
  <div class="contact-organ-donation" data-purpose="msg-contactdetails">
    <message-text :is-header="true">
      {{ $t('organ_donation.errors.contact.email') }}
    </message-text>
    <message-text>
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
};
</script>

<style>
</style>
