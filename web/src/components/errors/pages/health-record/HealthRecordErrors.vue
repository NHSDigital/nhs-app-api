<template>
  <div>

    <div v-if="hasRetried">
      <error-page id="healthRecordGpSessionError"
                  header-locale-ref="gpSessionErrors.healthRecord.healthRecordUnavailable"
                  :code="error.serviceDeskReference"
                  :update-header="false"
                  :show-back-link="false">
        <template v-slot:content>
          <p>{{ $t('gpSessionErrors.healthRecord.ifYouNeedInformationNow') }}</p>
          <contact-111 :text="$t('gpSessionErrors.healthRecord.forUrgentMedicalAdvice')"/>
        </template>
      </error-page>

      <h2 v-if="showPkb"
          id="otherThingsToDo">{{ $t('gpSessionErrors.healthRecord.otherThingsYouCanDo') }}</h2>

      <menu-item-list>
        <third-party-jump-off-button v-if="showPkbCarePlans"
                                     id="btn_pkb_care_plans"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider
                                       .pkb.carePlans" />
        <third-party-jump-off-button v-if="showPkbHealthTracker"
                                     id="btn_pkb_health_trackers"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider
                                       .pkb.healthTrackers" />
      </menu-item-list>
    </div>

    <shutter-container id="shutter-dialog-599">
      <error-title title="gpSessionErrors.healthRecord.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.healthRecord.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgain" @click="tryAgain" />
    </shutter-container>

  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import ShutterContainer from '@/components/shutters/ShutterContainer';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorPage from '@/components/errors/ErrorPage';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import { redirectTo, gpSessionErrorHasRetried } from '@/lib/utils';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'HealthRecordErrors',
  components: {
    Contact111,
    ShutterContainer,
    ErrorButton,
    ErrorPage,
    ErrorParagraph,
    ErrorTitle,
    MenuItemList,
    ThirdPartyJumpOffButton,
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
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  computed: {
    showPkbCarePlans() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'carePlans',
        },
      });
    },
    showPkbHealthTracker() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'healthTrackers',
        },
      });
    },
    showPkb() {
      return this.showPkbCarePlans ||
        this.showPkbHealthTracker;
    },
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

