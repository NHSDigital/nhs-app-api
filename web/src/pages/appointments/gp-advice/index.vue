<template>
  <div v-if="showTemplate" :class="!isNativeApp && $style.desktopWeb">
    <message-dialog v-if="isError" role="alert">
      <message-text data-purpose="error-heading"
                    :is-header="true">
        {{ $t('appointments.admin_help.errors.header') }}
      </message-text>
      <message-text data-purpose="reason-error"
                    :aria-label="$t('appointments.admin_help.errors.message.label')">
        {{ $t('appointments.admin_help.errors.message.text') }}
      </message-text>
    </message-dialog>
    <orchestrator v-else :provider="provider" :service-definition-id="serviceDefinitionId"/>
  </div>
</template>

<script>
import { get } from 'lodash/fp';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { noJsParameterName } from '@/lib/noJs';
import { isAnswerValid } from '@/lib/online-consultations/answer-validators';
import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import { INDEX, APPOINTMENT_GP_ADVICE_CONDITIONS } from '@/lib/routes';

export default {
  components: {
    MessageDialog,
    MessageText,
    Orchestrator,
  },
  computed: {
    isError() {
      return this.$store.state.onlineConsultations.error;
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
  async asyncData({ store, req, redirect, route }) {
    if (get('state.serviceJourneyRules.cdssAdmin.provider', store) === 'none') {
      redirect(302, INDEX.path, null);
    }
    const body = get('body', req);
    const question = get('state.onlineConsultations.question', store);
    const serviceDefinitionId = route.query.serviceDefinitionId ||
        store.state.onlineConsultations.gpAdviceServiceDefinitionId;

    if (!serviceDefinitionId) {
      return redirect(302, APPOINTMENT_GP_ADVICE_CONDITIONS.path, null);
    }

    const actionParams = {
      provider: store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      serviceDefinitionId,
      addJavascriptDisabledHeader: false,
    };

    if (get(noJsParameterName, body) !== undefined && question !== undefined) {
      actionParams.addJavascriptDisabledHeader = process.server;
      const answer = getAnswerFromRequestBody(body, question);
      await store.dispatch('onlineConsultations/setAnswer', answer);
      await store.dispatch('onlineConsultations/setAnswerIsValid', isAnswerValid(answer, question));
      await store.dispatch('onlineConsultations/setValidationError');
    }

    if (question === undefined) {
      await store.dispatch('onlineConsultations/getServiceDefinition', actionParams);
    } else if (store.state.onlineConsultations.answerIsValid) {
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', actionParams);
    }

    const previousClicked = get('direction', body) === 'back';

    if (previousClicked) {
      await store.dispatch('onlineConsultations/setPrevious');
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', actionParams);
    }

    return actionParams;
  },
  beforeDestroy() {
    this.$store.dispatch('onlineConsultations/clear', true);
  },
};
</script>
<style module lang="scss" scoped>
  div.desktopWeb {
    max-width: 540px;
  }
</style>
