<template>
  <div data-sid="welcome-info-proxy">
    <dl class="nhsuk-summary-list nhsuk-summary-list--no-border
               nhs-app-summary-list-inline">
      <div class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.name') }}:
        </dt>
        <dd class="nhsuk-summary-list__value" data-sid="proxy-name" data-hj-suppress>
          {{ proxyDetails.fullName }}
        </dd>
      </div>
      <div class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.age') }}:
        </dt>
        <dd class="nhsuk-summary-list__value" data-sid="proxy-user-age">
          {{ proxyAge }}
        </dd>
      </div>
      <div class="nhsuk-summary-list__row">
        <dt class="nhsuk-summary-list__key">
          {{ $t('home.gpSurgery') }}:
        </dt>
        <dd class="nhsuk-summary-list__value" data-sid="proxy-user-surgery">
          {{ proxyDetails.gpPracticeName }}
        </dd>
      </div>
    </dl>
  </div>
</template>

<script>

export default {
  name: 'ProxyWelcomeSection',
  props: {
    proxyDetails: {
      type: Object,
      default: null,
    },
    proxyAge: {
      type: String,
      default: '',
    },
  },
};
</script>
