<template>
  <div :class="[$style.welcomeInfo, !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-sid="welcome-info">
    <p v-if="name" class="nhsuk-u-margin-bottom-0">
      <strong>Name:</strong>
      <span data-sid="user-name" data-hj-suppress>{{ name }}</span>
      <br>
    </p>
    <p v-if="dateOfBirth" class="nhsuk-u-margin-bottom-0">
      <strong>Date of birth:</strong>
      <span data-sid="user-date-of-birth">{{ dateOfBirth | longDate }}</span>
      <br>
    </p>
    <p v-if="nhsNumber" id="welcomeSectionNhsNumber" class="nhsuk-u-margin-bottom-0">
      <strong>NHS number:</strong>
      <generic-voice-over-text-split :class="$style.fieldValue"
                                     :text="nhsNumber"
                                     :data-sid="'user-nhs-number'"/>
    </p>
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

<style module lang="scss">
  .welcomeInfo {
    padding-bottom: 1em;
  }
</style>
