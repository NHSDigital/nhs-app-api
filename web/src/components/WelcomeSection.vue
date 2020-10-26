<template>
  <div data-sid="welcome-info">
    <dl class="nhsuk-summary-list nhsuk-summary-list--no-border
               nhs-app-summary-list-inline nhsuk-u-margin-bottom-3">
      <div v-if="name" class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.name') }}:
        </dt>
        <dd class="nhsuk-summary-list__value" data-sid="user-name" data-hj-suppress>
          {{ name }}
        </dd>
      </div>
      <div class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.dateOfBirth') }}:
        </dt>
        <dd class="nhsuk-summary-list__value" data-sid="user-date-of-birth">
          {{ dateOfBirth | longDate }}
        </dd>
      </div>
      <div v-if="nhsNumber" id="welcomeSectionNhsNumber" class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.nhsNumber') }}:
        </dt>
        <dd class="nhsuk-summary-list__value">
          <generic-voice-over-text-split :text="nhsNumber"
                                         data-sid="user-nhs-number"/>
        </dd>
      </div>
    </dl>
  </div>
</template>

<script>
import GenericVoiceOverTextSplit from './widgets/GenericVoiceOverTextSplit';

export default {
  name: 'WelcomeSection',
  components: { GenericVoiceOverTextSplit },
  props: {
    dateOfBirth: {
      type: String,
      default: '',
    },
    name: {
      type: String,
      default: '',
    },
    nhsNumber: {
      type: String,
      default: '',
    },
  },
};
</script>
