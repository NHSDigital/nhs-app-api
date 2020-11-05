<template>
  <div :id="$style.serverError" class="pull-content">
    <message-dialog message-type="error">
      <message-text :is-header="true" data-purpose="error-heading">
        {{ $t('thirdPartyProviders.errors.silverIntegrationNotAvailable.heading') }}
      </message-text>
      <message-text data-purpose="contact">
        {{ $t('thirdPartyProviders.errors.silverIntegrationNotAvailable.contactYourGpSurgery') }}
      </message-text>
      <message-text data-purpose="go-to-nhs-app-home">
        <router-link :to="emptyIndexPath">
          {{ $t('thirdPartyProviders.errors.silverIntegrationNotAvailable.goToNHSAppHome') }}
        </router-link>
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { EMPTY_PATH } from '@/router/paths';

export default {
  name: 'NotFoundPage',
  components: {
    MessageDialog,
    MessageText,
  },
  props: {
    error: {
      type: Object,
      default() {
        return {};
      },
    },
  },
  data() {
    return {
      emptyIndexPath: EMPTY_PATH,
    };
  },
  async mounted() {
    const feature = this.$router.currentRoute.query.featureName;
    EventBus.$emit(UPDATE_HEADER, feature, true, true);
    EventBus.$emit(UPDATE_TITLE, feature, true);
  },
};
</script>
<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
</style>
<style module lang="scss" scoped>
  @import '../../style/buttons';
</style>
