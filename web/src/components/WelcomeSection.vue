<template>
  <div :class="$style.welcomeInfo" data-sid="welcome-info">
    <p>
      <strong>Date of birth:</strong>
      <span data-sid="user-date-of-birth">
        {{ $store.state.auth.user.dateOfBirth | shortDate }}
      </span>
    </p>
    <p>
      <strong>NHS number:</strong>
      <span data-sid="user-nhs-number">
        {{ $store.state.auth.user.nhsNumber }}
      </span>
    </p>
  </div>
</template>

<script>
export default {
  name: 'WelcomeSection',
};
</script>

<style module lang="scss">
  .welcomeInfo {
    padding-left: 1em;
    padding-bottom: 1em;
  }
</style>
