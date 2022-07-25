<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <form-error-summary v-if="showError"
                          :errors="errorText"
                          :errors-ids="`notifications-${getFirstChoiceValue('choices')}`"/>
      <p>{{ $t('notifications.weUseNotifications') }}</p>
      <p>{{ $t('notifications.theNhsAndConnected') }}</p>
      <nhs-uk-radio-group v-model="selectedValue"
                          name="notifications"
                          :heading="'<h2>'
                            + $t('notifications.doYouWantToGetNotifications')
                            + '</h2>'"
                          :heading-as-html="true"
                          :legend-size="mediumLegendSize"
                          :enable-error-dialog="false"
                          :error="showError"
                          :error-text="errorText"
                          :required="true"
                          :items="choices"/>
      <collapsible-details id="age-info">
        <template slot="header">
          {{ $t('notifications.aboutNotifications') }}
        </template>
        <p>{{ $t('notifications.ifYouWantToGetNotifications') }}</p>
        <p>{{ $t('notifications.ifYouShareThisDevice') }}</p>
      </collapsible-details>
      <p>
        {{ $t('notifications.moreInformation')
        }}<analytics-tracked-tag
          :href="privacyUrl"
          :text="$t('notifications.notificationsLink')"
          class="inline"
          tag="a"
          target="_blank">{{
            $t('notifications.notificationsLink')
          }}</analytics-tracked-tag>.
      </p>
      <primary-button id="btn_continue" @click="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </no-return-flow-layout>
</template>

<script>
import isUndefined from 'lodash/fp/isUndefined';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';
import FormErrorSummary from '@/components/FormErrorSummary';
import LegendSize from '@/lib/legend-size';
import NativeApp from '@/services/native-app';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectMixin from '@/components/RedirectMixin';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import get from 'lodash/fp/get';

export default {
  name: 'Index',
  components: {
    AnalyticsTrackedTag,
    CollapsibleDetails,
    FormErrorSummary,
    NhsUkRadioGroup,
    NoReturnFlowLayout,
    PrimaryButton,
  },
  mixins: [RedirectMixin],
  data() {
    return {
      choices: [
        {
          label: this.$t('notifications.turnOnNotifications'),
          hint: { text: this.$t('notifications.tellMeAbout') },
          value: 'yes',
        },
        {
          label: this.$t('notifications.doNotSendNotifications'),
          hint: { text: this.$t('notifications.iUnderstandIWillNotBeTold') },
          value: 'no',
        },
      ],
      errorText: this.$t('notifications.chooseIfYouWantToGetNotifications'),
      hasTriedToContinue: false,
      mediumLegendSize: LegendSize.Medium,
      privacyUrl: this.$store.$env.PRIVACY_POLICY_URL,
      selectedValue: undefined,
    };
  },
  computed: {
    showError() {
      return this.hasTriedToContinue && isUndefined(this.selectedValue);
    },
  },
  created() {
    const { notificationCookieExists, registered, notificationCommunicationError }
      = this.$store.state.notifications;

    if (!notificationCookieExists && registered) {
      this.$store.dispatch('notifications/addNotificationCookie');
    }

    if (notificationCookieExists || registered || notificationCommunicationError) {
      this.$store.dispatch('notifications/logMetrics', {
        screenShown: false,
        notificationsRegistered: notificationCookieExists || registered,
        didErrorAttemptingToUpdateStatus: false,
        ignoreError: true,
      });
      this.conditionalRedirect();
    }
  },
  mounted() {
    NativeApp.showHeaderSlim();
    NativeApp.hideWhiteScreen();
  },
  methods: {
    async onContinue() {
      this.hasTriedToContinue = true;
      if (this.showError) {
        EventBus.$emit(FOCUS_ERROR_ELEMENT);
        return;
      }

      await this.$store.dispatch('notifications/addNotificationCookie');

      if (this.selectedValue === 'yes') {
        await this.$store.dispatch('notifications/toggle');
        await this.$store.dispatch('notifications/logAudit', {
          notificationsRegistered: true,
          notificationsDecisionSource: 'Prompt',
        });
      } else {
        await this.$store.dispatch('notifications/logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: false,
          ignoreError: true,
        });
        await this.$store.dispatch('notifications/logAudit', {
          notificationsRegistered: false,
          notificationsDecisionSource: 'Prompt',
        });
      }

      if (!this.$store.state.notifications.notificationCommunicationError) {
        this.conditionalRedirect();
      }
    },
    getFirstChoiceValue(choicesName) {
      if (get(`${choicesName}[0].value`, this) !== undefined && get(`${choicesName}[0].value`, this) !== '') {
        return get(`${choicesName}[0].value`, this);
      }
      if (get(`${choicesName}[0].code`, this) !== undefined && get(`${choicesName}[0].code`, this) !== '') {
        return get(`${choicesName}[0].code`, this);
      }
      return '';
    },
  },
};
</script>
