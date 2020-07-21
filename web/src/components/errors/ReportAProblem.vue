<template>
  <div>
    <p class="nhsuk-u-font-size-16">
      <error-link class="nhsuk-u-margin-bottom-0 nhsuk-u-font-size-16"
                  from="errors.reportAProblemLink"
                  :action="contactUsUrl"
                  target="_blank"
                  :query-param="referenceParam"/>
      {{ this.$t('errors.referenceCode', { reference }) }}
    </p>
  </div>
</template>
<script>
import ErrorLink from '@/components/errors/ErrorLink';

export default {
  name: 'ReportAProblem',
  components: {
    ErrorLink,
  },
  props: {
    reference: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      contactUsUrl: this.$store.app.$env.CONTACT_US_URL,
    };
  },
  computed: {
    referenceParam() {
      return {
        param: 'errorcode',
        value: this.reference,
      };
    },
  },
};
</script>
