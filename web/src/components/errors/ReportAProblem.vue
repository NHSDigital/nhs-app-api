<template>
  <div class="nhsuk-u-margin-bottom-3 nhsuk-u-font-size-16">
    <error-link class="nhsuk-u-margin-bottom-0 nhsuk-u-font-size-16"
                from="apiErrors.reportAProblem"
                :action="contactUsUrl"
                target="_blank"
                :query-param="referenceParam"/>
    {{ this.$t('apiErrors.referenceReference', { reference }) }}
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
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
    };
  },
  computed: {
    referenceParam() {
      return {
        ErrCodeParam: 'errorcode',
        ErrCodeValue: this.reference,
        OdsCodeParam: 'odscode',
        OdsCodeValue: !this.$store.state.session.gpOdsCode ? '' : this.$store.state.session.gpOdsCode,
      };
    },
  },
};
</script>
