<template>
  <div v-if="showTemplate">
    <p>
      {{ $t('exampleNotifications.mainHeading') }}
    </p>
    <h2 class="nhsuk-heading-s nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0">
      {{ $t('exampleNotifications.nhsAppNotificationHeading') }}
    </h2>
    <img class="nhsuk-image__img nhsuk-u-margin-top-0" :alt="$t('exampleNotifications.nhsappNotificationImageAlt')"
         src="@/assets/nhsapp-notification-example.png" >
    <h2 class="nhsuk-heading-s nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-margin-top-1">
      {{ $t('exampleNotifications.gpMessageNotificationHeading') }}
    </h2>
    <img class="nhsuk-image__img nhsuk-u-margin-top-0" :alt="$t('exampleNotifications.gpMessageNotificationImageAlt')"
         src="@/assets/gpmessage-notification-example.png" >
  </div>
</template>

<script>
export default {
  name: 'ExampleNotificationsPage',
  layout: 'nhsuk-layout',
};
</script>
