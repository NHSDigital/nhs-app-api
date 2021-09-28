<template>
  <p>
    <a :href="actionUrl"
       rel="noopener noreferrer"
       target="_blank"
       :aria-label="messageLabel"
       data-purpose="main-back-button"
       @click.stop.prevent="onClick">
      {{ $t('login.authReturn.contactUsIfYouKeepSeeing.text', { errorCode }) }}
    </a>
  </p>
</template>

<script>
import get from 'lodash/fp/get';

export default {
  name: 'ServiceDeskReference',
  computed: {
    actionUrl() {
      return `${this.$store.$env.CONTACT_US_URL}?errorcode=${this.errorCode}`;
    },
    errorCode() {
      return get('$store.state.errors.apiErrors[0].serviceDeskReference')(this) || '';
    },
    messageLabel() {
      return `${this.$t('login.authReturn.contactUsIfYouKeepSeeing.label')}${this.errorCode.split('')}`;
    },
  },
  methods: {
    onClick() {
      window.open(this.actionUrl, '');
    },
  },
};
</script>
