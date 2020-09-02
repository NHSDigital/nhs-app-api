<template>
  <div>
    <error-title v-if="updateHeader" :title="headerLocaleRef" />
    <slot name="content"/>
    <hr>
    <report-a-problem v-if="hasReferenceCode" :reference="code"/>
    <slot name="actions"/>
    <error-link from="generic.back"
                :action="backUrl"
                data-purpose="error"
                :desktop-only="true"/>
  </div>
</template>
<script>
import ReportAProblem from '@/components/errors/ReportAProblem';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorTitle from '@/components/errors/ErrorTitle';

export default {
  name: 'GenericErrorScreen',
  components: {
    ReportAProblem,
    ErrorLink,
    ErrorTitle,
  },
  props: {
    code: {
      type: String,
      default: undefined,
    },
    updateHeader: {
      type: Boolean,
      default: true,
    },
    area: {
      type: String,
      default: undefined,
    },
    backUrl: {
      type: String,
      default: undefined,
    },
    headerLocaleRef: {
      type: String,
      default: undefined,
      required: true,
    },
  },
  computed: {
    hasReferenceCode() {
      return this.code !== '' && this.code !== undefined;
    },
  },
};
</script>
<style module lang="scss" scoped>
 hr {
  margin: 0.5em auto 0.5em;
 }
</style>
