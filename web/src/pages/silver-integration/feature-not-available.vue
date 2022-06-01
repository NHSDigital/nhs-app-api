<template>
  <div>

    <div class="pull-content">
      <message-dialog-generic override-style="plain">
        <message-text :is-header="true" data-purpose="content">
          <h1>{{ $t('thirdPartyProviders.information.silverIntegrationNotAvailable.heading',
                    {featureName}) }}</h1>
          <span>{{
            $t('thirdPartyProviders.information.silverIntegrationNotAvailable.youDoNotHaveAccess',
               {providerName }) }}</span>
        </message-text>
        <message-text data-purpose="go-to-nhs-app-home">
          <router-link :to="emptyIndexPath">
            {{ $t('thirdPartyProviders.information.silverIntegrationNotAvailable.goToNHSAppHome') }}
          </router-link>
        </message-text>
      </message-dialog-generic>
    </div>
  </div>
</template>

<script>
import MessageText from '@/components/widgets/MessageText';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { EMPTY_PATH } from '@/router/paths';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';

export default {
  name: 'NotFoundPage',
  components: {
    MessageText,
    MessageDialogGeneric,
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
      providerName: '',
      featureName: '',
    };
  },
  async mounted() {
    const feature = this.$router.currentRoute.query.featureName;
    this.featureName = this.$router.currentRoute.query.featureName;
    this.providerName = this.$router.currentRoute.query.providerName;
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
