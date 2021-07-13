<template>
  <div>
    <error-page v-if="hasRetried"
                id="healthRecordGpSessionError"
                header-locale-ref="gpSessionErrors.healthRecord.healthRecordUnavailable"
                :code="error.serviceDeskReference"
                :update-header="true"
                :show-back-link="false">
      <template v-slot:content>
        <p>{{ $t('gpSessionErrors.healthRecord.ifYouNeedInformationNow') }}</p>
        <contact-111 :text="$t('gpSessionErrors.healthRecord.forUrgentMedicalAdvice')"/>
      </template>
    </error-page>

    <error-container v-else id="error-dialog-599">
      <error-title title="gpSessionErrors.healthRecord.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.healthRecord.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </error-container>

  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';

export default {
  name: 'HealthRecordErrors',
  components: {
    Contact111,
    ErrorButton,
    ErrorContainer,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
  },
  props: {
    error: {
      type: Object,
      default: undefined,
      required: true,
    },
    referenceCode: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      hasRetried: gpSessionErrorHasRetried(),
    };
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      this.hasRetried = gpSessionErrorHasRetried();
    },
  },
  methods: {
    tryAgain() {
      sessionStorage.setItem('hasRetried', true);
      redirectTo(this, GP_MEDICAL_RECORD_PATH, { hr: true }, true);
    },
  },
};
</script>
