<template>
  <div :class="[$style.welcomeInfo, !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-sid="welcome-info">
    <p v-if="name">
      <strong>Name:</strong>
      <span data-sid="user-name" data-hj-suppress>
        {{ name }}
      </span>
    </p>
    <p v-if="dateOfBirth">
      <strong :class="$style.fieldName" >Date of birth:</strong>
      <span :class="$style.fieldValue" data-sid="user-date-of-birth">
        {{ dateOfBirth | longDate }}
      </span>
    </p>
    <p v-if="nhsNumber">
      <strong :class="$style.fieldName">NHS number:</strong>
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
    name: {
      type: String,
      default: '',
    },
    dateOfBirth: {
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
  @import "../style/fonts";

  div {
   &.desktopWeb {
    .fieldName {
     font-family: $default-web;
     font-weight: 500;
     font-size: 1em;
     line-height: 1.5em;
     color: #425563;
    }

    .fieldValue {
     font-family: $default-web;
     font-weight: lighter;
     font-size: 1em;
     line-height: 1.5em;
     color: #425563;
    }
   }

   .fieldName {
   }

   .fieldValue {
   }
  }

 .welcomeInfo {
  padding-bottom: 1em;
 }
</style>
