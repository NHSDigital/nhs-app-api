import { AppPage } from './constants';

window.nhsapp = {
  navigation: {
    goToHomePage() {
      // TODO: remove when min app version is 1.36
      if (window.nhsappNative.goToHomepage) {
        window.nhsappNative.goToHomepage();
      } else {
        this.goToPage(AppPage.HOME_PAGE);
      }
    },
    goToPage(appPage) {
      window.nhsappNative.goToPage(appPage);
    },
    openBrowserOverlay(url) {
      window.nhsappNative.openBrowserOverlay(url);
    },
    openExternalBrowser(url) {
      window.nhsappNative.openExternalBrowser(url);
    },
  },
  tools: {
    getAppPlatform() {
      if (navigator.userAgent.indexOf('nhsapp-android') !== -1) {
        return 'android';
      } if (navigator.userAgent.indexOf('nhsapp-ios') !== -1) {
        return 'ios';
      }
      return 'none';
    },
    isOpenInNHSApp: () => navigator.userAgent.indexOf('nhsapp') !== -1,
  },
  storage: {
    addEventToCalendar(subject, body, location, startTimeEpochInSeconds, endTimeEpochInSeconds) {
      // if end time is not defined, set it to the start time
      let validEndTimeEpochInSeconds = endTimeEpochInSeconds;
      if (!endTimeEpochInSeconds) {
        validEndTimeEpochInSeconds = startTimeEpochInSeconds;
      }

      const calendarEvent = JSON.stringify({
        subject,
        body,
        location,
        startTimeEpochInSeconds,
        endTimeEpochInSeconds: validEndTimeEpochInSeconds,
      });
      window.nhsappNative.addEventToCalendar(calendarEvent);
    },
    downloadFromBytes(base64Data, fileName, mimeType) {
      if (window.nhsappNative.startDownloadFromJson) {
        window.nhsappNative.startDownloadFromJson(
          JSON.stringify({
            base64Data,
            fileName,
            mimeType,
          }),
        );
        return;
      }

      window.nhsappNative.downloadFromBytes(base64Data, fileName, mimeType);
    },
  },
};

window.nhsapp.navigation.AppPage = AppPage;
