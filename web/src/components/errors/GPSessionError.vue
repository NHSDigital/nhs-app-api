<template>
  <div>
    <error-title :title="this.$t(`gpSessionErrors.${area}.header`)" />
    <slot name="content"/>
    <hr>
    <report-a-problem :reference="code"/>
    <h2 class="nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-3 nhsuk-u-margin-bottom-0">
      {{ this.$t('gpSessionErrors.whatYouCanDo') }}
    </h2>
    <menu-item-list>
      <slot name="items"/>
    </menu-item-list>
    <error-link from="generic.backButton.text"
                :action="backUrl"
                data-purpose="error"
                :desktop-only="true"/>
  </div>
</template>
<script>
import ReportAProblem from '@/components/errors/ReportAProblem';
import MenuItemList from '@/components/MenuItemList';
import ErrorTitle from '@/components/errors/ErrorTitle';
import ErrorLink from '@/components/errors/ErrorLink';

export default {
  name: 'GpSessionError',
  components: {
    ReportAProblem,
    MenuItemList,
    ErrorTitle,
    ErrorLink,
  },
  props: {
    code: {
      type: String,
      default: undefined,
    },
    area: {
      type: String,
      default: undefined,
    },
    backUrl: {
      type: String,
      default: undefined,
    },
  },
};
</script>
<style module lang="scss" scoped>
 hr {
  margin: 0.5em auto 0.5em;
 }
</style>
