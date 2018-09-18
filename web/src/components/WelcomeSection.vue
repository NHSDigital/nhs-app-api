<template>
  <div :class="$style.welcomeInfo" data-sid="welcome-info">
    <p v-if="name">
      <strong>Name:</strong>
      <span data-sid="user-name">
        {{ name }}
      </span>
    </p>
    <p v-if="dateOfBirth">
      <strong>Date of birth:</strong>
      <span data-sid="user-date-of-birth">
        {{ dateOfBirth | shortDate }}
      </span>
    </p>
    <p v-if="nhsNumber">
      <strong>NHS number:</strong>
      <span data-sid="user-nhs-number">
        {{ nhsNumber }}
      </span>
    </p>
  </div>
</template>

<script>
export default {
  name: 'WelcomeSection',
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
  .welcomeInfo {
    padding-left: 1em;
    padding-bottom: 1em;
  }
</style>
