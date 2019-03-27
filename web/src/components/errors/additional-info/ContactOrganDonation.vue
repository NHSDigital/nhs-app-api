<template>
  <div class="contact-organ-donation">
    <message-text :is-header="true">
      {{ $t('organ_donation.errors.contact.phone') }}
    </message-text>
    <message-text>
      <span>0300 123 23 23</span>
    </message-text>
    <message-text :is-header="true">
      {{ $t('organ_donation.errors.contact.email') }}
    </message-text>
    <message-text>
      <a href="mailto:NHSApp.Enquiries@nhsbt.nhs.uk">NHSApp.Enquiries@nhsbt.nhs.uk</a>
    </message-text>
    <message-text>
      {{ $t('organ_donation.errors.contact.otherWays') }}
      <br>
      <a id="contactUs" :href="contactUs">
        {{ contactUsText }}
      </a>
    </message-text>
  </div>
</template>

<script>
import MessageText from '@/components/widgets/MessageText';
import isString from 'lodash/fp/isString';

export default {
  name: 'ContactOrganDonation',
  components: {
    MessageText,
  },
  data() {
    return {
      contactUs: this.$store.app.$env.ORGAN_DONATION_CONTACT_US_URL,
    };
  },
  computed: {
    contactUsText() {
      return isString(this.contactUs) ? this.contactUs.replace('https://www.', '') : '';
    },
  },
};
</script>

<style>
</style>
